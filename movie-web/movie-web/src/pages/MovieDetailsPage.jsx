import { useEffect, useState } from "react"
import { useParams } from "react-router-dom"
import api from "../services/api"

function MovieDetailsPage() {

    const { id } = useParams()

    const [movie, setMovie] = useState(null)
    const [rating, setRating] = useState(5)
    const [message, setMessage] = useState("")

    useEffect(() => {

        const fetchMovie = async () => {

            try {

                const response = await api.get(`/Movie/${id}/details`)

                setMovie(response.data)

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

                <p>{message}</p>

            </div>

        </div>
    )
}

export default MovieDetailsPage