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
        <div className="profile-page">
            <div className="profile-card">
                <h1>Profile</h1>

                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label>Ad Soyad</label>
                        <input
                            name="fullName"
                            placeholder="Örn: Eylem Fidan"
                            value={profile.fullName}
                            onChange={handleChange}
                        />
                    </div>

                    <div className="form-group">
                        <label>Favori Türler</label>
                        <input
                            name="favoriteGenres"
                            placeholder="Örn: Comedy|Action|Drama"
                            value={profile.favoriteGenres}
                            onChange={handleChange}
                        />
                        <small>Türleri | işaretiyle ayır. Örn: Comedy|Action</small>
                    </div>

                    <div className="form-group">
                        <label>Dil Tercihi</label>
                        <input
                            name="languagePreference"
                            placeholder="Örn: English, Turkish"
                            value={profile.languagePreference}
                            onChange={handleChange}
                        />
                    </div>

                    <div className="form-group">
                        <label>Yerli / Yabancı Tercihi</label>
                        <input
                            name="localOrForeign"
                            placeholder="Örn: Foreign veya Local"
                            value={profile.localOrForeign}
                            onChange={handleChange}
                        />
                    </div>

                    <div className="form-group">
                        <label>İzleme Amacı</label>
                        <input
                            name="watchingPurpose"
                            placeholder="Örn: Entertainment, Learning"
                            value={profile.watchingPurpose}
                            onChange={handleChange}
                        />
                    </div>

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
        </div>
    )
}

export default ProfilePage