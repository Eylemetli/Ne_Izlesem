import { useEffect, useState } from "react"
import api from "../services/api"
import MovieCard from "../components/MovieCard"

function HomePage() {
    const [movies, setMovies] = useState([])
    const [genre, setGenre] = useState("")
    const [minRating, setMinRating] = useState("")
    const [searchText, setSearchText] = useState("")

    const fetchRecommendations = async () => {
        try {
            const response = await api.get("/Recommendation/me")
            setMovies(response.data)
        } catch (error) {
            console.log(error)
        }
    }

    useEffect(() => {
        fetchRecommendations()
    }, [])

    const handleFilter = async () => {
        try {
            const response = await api.get("/Movie/filter", {
                params: {
                    genre: genre,
                    minRating: minRating
                }
            })

            setMovies(response.data)
        } catch (error) {
            console.log(error)
        }
    }

    const handleSearch = async () => {
        try {
            const response = await api.get("/Movie/search", {
                params: {
                    query: searchText
                }
            })

            setMovies(response.data)
        } catch (error) {
            console.log(error)
        }
    }

    return (
        <div>
            <h1>Home Page</h1>

            <div style={{ padding: "20px" }}>
                <input
                    type="text"
                    placeholder="Film ara..."
                    value={searchText}
                    onChange={(e) => setSearchText(e.target.value)}
                />

                <button onClick={handleSearch}>
                    Ara
                </button>

                <select
                    value={genre}
                    onChange={(e) => setGenre(e.target.value)}
                >
                    <option value="">Tüm Türler</option>
                    <option value="Action">Action</option>
                    <option value="Comedy">Comedy</option>
                    <option value="Drama">Drama</option>
                    <option value="Adventure">Adventure</option>
                    <option value="Thriller">Thriller</option>
                    <option value="Romance">Romance</option>
                    <option value="Sci-Fi">Sci-Fi</option>
                </select>

                <select
                    value={minRating}
                    onChange={(e) => setMinRating(e.target.value)}
                >
                    <option value="">Minimum Puan</option>
                    <option value="5">5+</option>
                    <option value="6">6+</option>
                    <option value="7">7+</option>
                    <option value="8">8+</option>
                </select>

                <button onClick={handleFilter}>
                    Filtrele
                </button>

                <button onClick={fetchRecommendations}>
                    Önerilere Dön
                </button>
            </div>

            <div
                style={{
                    display: "grid",
                    gridTemplateColumns: "repeat(auto-fill, minmax(220px, 1fr))",
                    gap: "20px",
                    padding: "20px"
                }}
            >
                {movies.map((movie) => (
                    <MovieCard key={movie.id} movie={movie} />
                ))}
            </div>
        </div>
    )
}

export default HomePage