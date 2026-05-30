import { Link, useNavigate } from "react-router-dom"

function Navbar() {

    const navigate = useNavigate()

    const token = localStorage.getItem("token")

    const logout = () => {

        localStorage.removeItem("token")
        localStorage.removeItem("userId")
        localStorage.removeItem("email")

        navigate("/login")
    }

    return (

        <nav style={{ padding: "20px" }}>

            {token ? (
                <>

                    <Link to="/">Home</Link> |{" "}

                    <Link to="/profile">Profile</Link> |{" "}

                    <Link to="/watchlist">Watchlist</Link> |{" "}

                    <button onClick={logout}>
                        Logout
                    </button>

                </>
            ) : (
                <>

                    <Link to="/login">Login</Link> |{" "}

                    <Link to="/register">Register</Link>

                </>
            )}

        </nav>
    )
}

export default Navbar