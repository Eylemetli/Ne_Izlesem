import { useEffect, useState } from "react"
import api from "../services/api"

function ProfilePage() {
    const [profile, setProfile] = useState({
        fullName: "",
        favoriteGenres: "",
        languagePreference: "",
        localOrForeign: "",
        watchingPurpose: ""
    })

    const [message, setMessage] = useState("")
    const [watchlist, setWatchlist] = useState([])

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const response = await api.get("/Profile/me")

                setProfile({
                    fullName: response.data.fullName || "",
                    favoriteGenres: response.data.favoriteGenres || "",
                    languagePreference: response.data.languagePreference || "",
                    localOrForeign: response.data.localOrForeign || "",
                    watchingPurpose: response.data.watchingPurpose || ""
                })
            } catch (error) {
                console.log(error)
            }
        }
        const fetchWatchlist = async () => {

            try {

                const response = await api.get(
                    `/Watchlist/${localStorage.getItem("userId")}`
                )

                setWatchlist(response.data)

            } catch (error) {

                console.log(error)
            }
        }

        fetchProfile()
        fetchWatchlist()
    }, [])

    const handleChange = (e) => {
        setProfile({
            ...profile,
            [e.target.name]: e.target.value
        })
    }

    const handleSubmit = async (e) => {
        e.preventDefault()

        try {
            await api.put("/Profile/me", profile)

            setMessage("Profil güncellendi.")
        } catch (error) {
            console.log(error)
            setMessage("Profil güncellenemedi.")
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
        <div>
            <h1>Profile</h1>

            <form onSubmit={handleSubmit}>
                <input
                    name="fullName"
                    placeholder="Ad Soyad"
                    value={profile.fullName}
                    onChange={handleChange}
                />

                <input
                    name="favoriteGenres"
                    placeholder="Favorite Genres örn: Comedy|Action"
                    value={profile.favoriteGenres}
                    onChange={handleChange}
                />

                <input
                    name="languagePreference"
                    placeholder="Language Preference"
                    value={profile.languagePreference}
                    onChange={handleChange}
                />

                <input
                    name="localOrForeign"
                    placeholder="Local or Foreign"
                    value={profile.localOrForeign}
                    onChange={handleChange}
                />

                <input
                    name="watchingPurpose"
                    placeholder="Watching Purpose"
                    value={profile.watchingPurpose}
                    onChange={handleChange}
                />

                <button type="submit">Profili Güncelle</button>
            </form>
            <h2>Watchlist</h2>

            <div
                style={{
                    display: "grid",
                    gridTemplateColumns: "repeat(auto-fill, minmax(200px, 1fr))",
                    gap: "20px"
                }}
            >
                {watchlist.map((movie) => (

                    <div key={movie.id}>

                        <img
                            src={movie.posterUrl}
                            alt={movie.title}
                            style={{
                                width: "100%",
                                borderRadius: "10px"
                            }}
                        />

                        <h3>{movie.title}</h3>
                        <button onClick={() => removeFromWatchlist(movie.id)}>
                            Kaldır
                        </button>

                    </div>
                ))}
            </div>

            <p>{message}</p>
        </div>
    )
}

export default ProfilePage