import { Link } from "react-router-dom"
function MovieCard({ movie }) {
    return (
        <Link
            to={`/movie/${movie.id}`}
            style={{ textDecoration: "none", color: "black" }}
        >
            <div
                onMouseEnter={(e) => {
                    e.currentTarget.style.transform = "scale(1.03)"
                }}

                onMouseLeave={(e) => {
                    e.currentTarget.style.transform = "scale(1)"
                }}
                style={{
                    border: "1px solid #ccc",
                    borderRadius: "10px",
                    padding: "10px",
                    transition: "0.3s",
                    cursor: "pointer"
                }}
            >
                {movie.posterUrl ? (
                    <img
                        src={movie.posterUrl}
                        alt={movie.title}
                        style={{
                            width: "100%",
                            height: "350px",
                            objectFit: "contain",
                            backgroundColor: "#111",
                            borderRadius: "10px"
                        }}
                    />
                ) : (
                    <div
                        style={{
                            height: "350px",
                            backgroundColor: "#111",
                            color: "white",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            borderRadius: "10px"
                        }}
                    >
                        Poster Yok
                    </div>
                )}

                <h3>{movie.title}</h3>

                <p>{movie.genres}</p>
                <p>
                    {movie.overview?.slice(0, 100)}...
                </p>

                <p>⭐ {movie.voteAverage}</p>
            </div>
        </Link>
    )
}

export default MovieCard