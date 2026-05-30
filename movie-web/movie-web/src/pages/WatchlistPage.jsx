import { useEffect, useState } from "react"
import api from "../services/api"
import MovieCard from "../components/MovieCard"

function WatchlistPage() {
    const [watchlist, setWatchlist] = useState([])
    const [loading, setLoading] = useState(false)

    useEffect(() => {
        fetchWatchlist()
    }, [])

    const fetchWatchlist = async () => {
        try {
            setLoading(true)
            const response = await api.get(
                `/Watchlist/${localStorage.getItem("userId")}`
            )
            setWatchlist(response.data)
        } catch (error) {
            console.log(error)
        } finally {
            setLoading(false)
        }
    }

    const removeFromWatchlist = async (movieId) => {
        try {
            await api.delete(
                `/Watchlist?userId=${localStorage.getItem("userId")}&movieId=${movieId}`
            )
            setWatchlist(watchlist.filter((movie) => movie.id !== movieId))
        } catch (error) {
            console.log(error)
        }
    }

    return (
        <div style={{ padding: "20px" }}>
            <h1>Watchlist</h1>

            {loading && <h2>Loading...</h2>}

            <div style={{
                display: "grid",
                gridTemplateColumns: "repeat(auto-fill, minmax(220px, 1fr))",
                gap: "20px"
            }}>
                {watchlist.map((movie) => (
                    <div key={movie.id}>
                        <MovieCard movie={movie} />
                        <button onClick={() => removeFromWatchlist(movie.id)}>
                            Kaldır
                        </button>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default WatchlistPage