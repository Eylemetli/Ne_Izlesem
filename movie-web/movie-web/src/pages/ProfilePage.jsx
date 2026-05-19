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

        fetchProfile()
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

            <p>{message}</p>
        </div>
    )
}

export default ProfilePage