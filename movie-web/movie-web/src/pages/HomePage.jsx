import { useEffect, useState } from "react"
import api from "../services/api"
import MovieCard from "../components/MovieCard"

function HomePage() {
    const [movies, setMovies] = useState([])

    useEffect(() => {
        const fetchMovies = async () => {
            try {
                const response = await api.get("/Recommendation/me")
                setMovies(response.data.slice(0, 20))
            } catch (error) {
                console.log(error)
            }
        }

        fetchMovies()
    }, [])

    return (
        <div>
            <h1>Home Page</h1>

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