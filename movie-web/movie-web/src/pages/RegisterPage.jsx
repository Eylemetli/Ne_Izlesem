import { useState } from "react"
import { useNavigate } from "react-router-dom"
import api from "../services/api"

function RegisterPage() {
    const [fullName, setFullName] = useState("")
    const [email, setEmail] = useState("")
    const [password, setPassword] = useState("")
    const [message, setMessage] = useState("")

    const navigate = useNavigate()

    const handleRegister = async (e) => {
        e.preventDefault()

        try {
            await api.post("/Auth/register", {
                fullName,
                email,
                password
            })

            setMessage("Kayıt başarılı.")
            navigate("/login")
        } catch (error) {
            console.log(error)
            setMessage("Kayıt başarısız.")
        }
    }

    return (
        <div>
            <h1>Register</h1>

            <form onSubmit={handleRegister}>
                <input
                    type="text"
                    placeholder="Ad Soyad"
                    value={fullName}
                    onChange={(e) => setFullName(e.target.value)}
                />

                <input
                    type="email"
                    placeholder="Email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />

                <input
                    type="password"
                    placeholder="Şifre"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />

                <button type="submit">Kayıt Ol</button>
            </form>

            <p>{message}</p>
        </div>
    )
}

export default RegisterPage