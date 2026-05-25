import { Link } from "react-router-dom"
function MovieCard({ movie }) {
    return (
        <Link
            to={`/movie/${movie.id}`}
            style={{ textDecoration: "none", color: "white" }}
        >
            <div
                onMouseEnter={(e) => {
                    e.currentTarget.style.transform = "scale(1.03)"
                }}

                onMouseLeave={(e) => {
                    e.currentTarget.style.transform = "scale(1)"
                }}
                style={{
                    backgroundColor: "#1e1e1e",
                    borderRadius: "12px",
                    overflow: "hidden",
                    transition: "0.3s",
                    cursor: "pointer",
                    boxShadow: "0 4px 10px rgba(0,0,0,0.4)"
                }}
            >
                {movie.posterUrl ? (
                    <img
                        src={movie.posterUrl}
                        alt={movie.title}
                        style={{
                            width: "100%",
                            height: "330px",
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

                <div style={{ padding: "15px" }}>

                    <h3
                        style={{
                            marginTop: 0
                        }}
                    >
                        {movie.title}
                    </h3>

                    <p>{movie.genres}</p>

                    <p>
                        {movie.overview?.slice(0, 100)}...
                    </p>

                    <p>⭐ {movie.voteAverage}</p>

                </div>
            </div>
        </Link>
    )
}

export default MovieCard