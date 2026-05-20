import { useEffect, useState } from "react"
import { useParams } from "react-router-dom"
import api from "../services/api"
import MovieCard from "../components/MovieCard"

function MovieDetailsPage() {

    const { id } = useParams()

    const [movie, setMovie] = useState(null)
    const [rating, setRating] = useState(5)
    const [message, setMessage] = useState("")
    const [similarMovies, setSimilarMovies] = useState([])

    useEffect(() => {

        const fetchMovie = async () => {

            try {

                const response = await api.get(`/Movie/${id}/details`)

                setMovie(response.data)
                const similarResponse = await api.get(`/Movie/${id}/similar`)

                setSimilarMovies(similarResponse.data)

            } catch (error) {
                console.log(error)
            }
        }

        fetchMovie()

    }, [id])

    const submitRating = async () => {

        try {

            await api.post(
                `/Rating?userId=${localStorage.getItem("userId")}&movieId=${id}&rating=${Number(rating)}`
            )

            setMessage("Puan verildi.")

        } catch (error) {

            console.log(error)

            setMessage("Hata oluştu.")
        }
    }

    const addToWatchlist = async () => {

        try {

            await api.post(
                `/Watchlist?userId=${localStorage.getItem("userId")}&movieId=${id}`, null
            )

            alert("Watchliste eklendi.")

        } catch (error) {

            console.log(error)

            alert("Film zaten kayıtlı olabilir.")
        }
    }

    if (!movie) {
        return <h1>Loading...</h1>
    }

    return (
        <div style={{ padding: "20px" }}>

            <img
                src={movie.fullPosterUrl}
                alt={movie.title}
                style={{
                    width: "300px",
                    borderRadius: "10px"
                }}
            />

            <h1>{movie.title}</h1>

            <p>{movie.overview}</p>

            <p>⭐ {movie.vote_average}</p>

            <p>{movie.release_date}</p>
            <div>

                <select
                    value={rating}
                    onChange={(e) => setRating(e.target.value)}
                >
                    <option value="1">1</option>
                    <option value="2">2</option>
                    <option value="3">3</option>
                    <option value="4">4</option>
                    <option value="5">5</option>
                </select>

                <button onClick={submitRating}>
                    Puan Ver
                </button>
                <button onClick={addToWatchlist}>
                    Watchliste Ekle
                </button>

                <p>{message}</p>

            </div>
            <h2>Benzer Filmler</h2>

            <div
                style={{
                    display: "grid",
                    gridTemplateColumns: "repeat(auto-fill, minmax(220px, 1fr))",
                    gap: "20px"
                }}
            >
                {similarMovies.map((movie) => (
                    <MovieCard key={movie.id} movie={movie} />
                ))}
            </div>

        </div>
    )
}

export default MovieDetailsPage