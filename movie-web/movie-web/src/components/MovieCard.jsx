import { Link } from "react-router-dom"
function MovieCard({ movie }) {
    return (
        <Link
            to={`/movie/${movie.id}`}
            style={{ textDecoration: "none", color: "black" }}
        >
            <div
                style={{
                    border: "1px solid #ccc",
                    borderRadius: "10px",
                    padding: "10px"
                }}
            >
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