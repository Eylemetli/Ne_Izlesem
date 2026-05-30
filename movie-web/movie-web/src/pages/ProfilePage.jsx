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

    const [genres, setGenres] = useState([])
    const [selectedGenres, setSelectedGenres] = useState([])
    const [message, setMessage] = useState("")

    useEffect(() => {
        fetchProfile()
        fetchGenres()
    }, [])

    const fetchGenres = async () => {
        try {
            const response = await api.get("/Movie/genres")
            setGenres(response.data)
        } catch (error) {
            console.log(error)
        }
    }

    const fetchProfile = async () => {
        try {
            const response = await api.get("/Profile/me")
            const data = response.data
            setProfile({
                fullName: data.fullName || "",
                favoriteGenres: data.favoriteGenres || "",
                languagePreference: data.languagePreference || "",
                localOrForeign: data.localOrForeign || "",
                watchingPurpose: data.watchingPurpose || ""
            })
            if (data.favoriteGenres) {
                setSelectedGenres(data.favoriteGenres.split("|"))
            }
        } catch (error) {
            console.log(error)
        }
    }

    const handleChange = (e) => {
        setProfile({ ...profile, [e.target.name]: e.target.value })
    }

    const toggleGenre = (genre) => {
        setSelectedGenres(prev =>
            prev.includes(genre)
                ? prev.filter(g => g !== genre)
                : [...prev, genre]
        )
    }

    const handleSubmit = async (e) => {
        e.preventDefault()
        try {
            await api.put("/Profile/me", {
                ...profile,
                favoriteGenres: selectedGenres.join("|")
            })
            setMessage("Profil güncellendi.")
        } catch (error) {
            console.log(error)
            setMessage("Profil güncellenemedi.")
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
                        <div style={{
                            display: "flex",
                            flexWrap: "wrap",
                            gap: "8px",
                            marginTop: "8px"
                        }}>
                            {genres.map(g => (
                                <button
                                    key={g}
                                    type="button"
                                    onClick={() => toggleGenre(g)}
                                    style={{
                                        padding: "6px 12px",
                                        borderRadius: "20px",
                                        border: "none",
                                        cursor: "pointer",
                                        backgroundColor: selectedGenres.includes(g) ? "#e50914" : "#333",
                                        color: "white",
                                        fontWeight: selectedGenres.includes(g) ? "bold" : "normal"
                                    }}
                                >
                                    {g}
                                </button>
                            ))}
                        </div>
                    </div>

                    <div className="form-group">
                        <label>Dil Tercihi</label>
                        <select name="languagePreference" value={profile.languagePreference} onChange={handleChange}>
                            <option value="">Seçin</option>
                            <option value="English">English</option>
                            <option value="Turkish">Turkish</option>
                            <option value="Other">Other</option>
                        </select>
                    </div>

                    <div className="form-group">
                        <label>Yerli / Yabancı Tercihi</label>
                        <select name="localOrForeign" value={profile.localOrForeign} onChange={handleChange}>
                            <option value="">Seçin</option>
                            <option value="Local">Local</option>
                            <option value="Foreign">Foreign</option>
                            <option value="Both">Both</option>
                        </select>
                    </div>

                    <div className="form-group">
                        <label>İzleme Amacı</label>
                        <select name="watchingPurpose" value={profile.watchingPurpose} onChange={handleChange}>
                            <option value="">Seçin</option>
                            <option value="Entertainment">Entertainment</option>
                            <option value="Learning">Learning</option>
                            <option value="Relaxation">Relaxation</option>
                            <option value="Other">Other</option>
                        </select>
                    </div>

                    <button type="submit">Profili Güncelle</button>
                </form>

                <p>{message}</p>
            </div>
        </div>
    )
}

export default ProfilePage