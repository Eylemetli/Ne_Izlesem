import { useEffect, useState } from "react"
import api from "../services/api"
import MovieCard from "../components/MovieCard"

function HomePage() {
    const [movies, setMovies] = useState([])
    const [discover, setDiscover] = useState([])
    const [genre, setGenre] = useState("")
    const [minRating, setMinRating] = useState("")
    const [searchText, setSearchText] = useState("")
    const [loading, setLoading] = useState(false)

    const fetchRecommendations = async () => {
        try {
            setLoading(true)

            const response = await api.get("/Recommendation/me")

            if (response.data.recommendations) {
                setMovies(response.data.recommendations)
                setDiscover(response.data.discover || [])
            } else {
                setMovies(response.data)
                setDiscover([])
            }

        } catch (error) {
            console.log(error)
        } finally {
            setLoading(false)
        }
    }
    useEffect(() => {
        fetchRecommendations()
    }, [])

    const handleFilter = async () => {
        try {
            setLoading(true)
            const response = await api.get("/Recommendation/me/filter", {
                params: {
                    genre: genre,
                    minRating: minRating
                }
            })
            setMovies(response.data)
            setDiscover([])
        } catch (error) {
            console.log(error)
        } finally {
            setLoading(false)
        }
    }

    const handleSearch = async () => {
        try {
            setLoading(true)
            const response = await api.get("/Movie/search", {
                params: {
                    query: searchText
                }
            })

            setMovies(response.data)
        } catch (error) {
            console.log(error)
        }
        finally {
            setLoading(false)
        }
    }

    return (
        <div>
            <div
                style={{
                    height: "350px",
                    backgroundImage:
                        "url('https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?q=80&w=2070')",
                    backgroundSize: "cover",
                    backgroundPosition: "center",
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    padding: "40px"
                }}
            >
                <h1
                    style={{
                        fontSize: "60px",
                        margin: 0
                    }}
                >
                    Movie Recommendation System
                </h1>

                <p
                    style={{
                        fontSize: "20px",
                        maxWidth: "600px"
                    }}
                >
                    Personalized movie recommendations powered by AI and user preferences.
                </p>
            </div>

            <h2 style={{ paddingLeft: "20px" }}>
                Recommended Movies
            </h2>
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

            {loading && <h2>Loading...</h2>}

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
            {discover.length > 0 && (
                <>
                    <h2 style={{ paddingLeft: "20px" }}>
                        Bunları da İzleyebilirsiniz
                    </h2>

                    <div
                        style={{
                            display: "grid",
                            gridTemplateColumns: "repeat(auto-fill, minmax(220px, 1fr))",
                            gap: "20px",
                            padding: "20px"
                        }}
                    >
                        {discover.map((movie) => (
                            <MovieCard key={movie.id} movie={movie} />
                        ))}
                    </div>
                </>
            )}
        </div>
    )
}

export default HomePage