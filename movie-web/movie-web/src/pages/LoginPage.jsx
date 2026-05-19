import { useState } from "react"
import { useNavigate } from "react-router-dom"
import api from "../services/api"

function LoginPage() {
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [message, setMessage] = useState("")

    const navigate = useNavigate()

    const handleLogin = async (e) => {
        e.preventDefault()

        try {
            const response = await api.post("/Auth/login", {
                email,
                password
            })

            localStorage.setItem("token", response.data.token)
            localStorage.setItem("userId", response.data.userId)
            localStorage.setItem("email", response.data.email)

            setMessage("Giriş başarılı.")

            navigate("/")
        } catch (error) {
            setMessage("Email veya şifre hatalı.")
            console.log(error)
        }
    }

    return (
        <div>
            <h1>Login</h1>

            <form onSubmit={handleLogin}>
                <div>
                    <input
                        type="email"
                        placeholder="Email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>

                <div>
                    <input
                        type="password"
                        placeholder="Şifre"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>

                <button type="submit">Giriş Yap</button>
            </form>

            <p>{message}</p>
        </div>
    )
}

export default LoginPage