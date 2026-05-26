import 'package:flutter/material.dart';
import '../services/api_service.dart';

class ProfilePage extends StatefulWidget {
  const ProfilePage({super.key});

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  final ApiService apiService = ApiService();

  final fullNameController = TextEditingController();
  final favoriteGenresController = TextEditingController();
  final languagePreferenceController = TextEditingController();
  final localOrForeignController = TextEditingController();
  final watchingPurposeController = TextEditingController();

  String message = "";
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    fetchProfile();
  }

  Future<void> fetchProfile() async {
    try {
      final data = await apiService.getProfile();

      fullNameController.text = data["fullName"] ?? "";
      favoriteGenresController.text = data["favoriteGenres"] ?? "";
      languagePreferenceController.text = data["languagePreference"] ?? "";
      localOrForeignController.text = data["localOrForeign"] ?? "";
      watchingPurposeController.text = data["watchingPurpose"] ?? "";

      setState(() {
        isLoading = false;
      });
    } catch (e) {
      setState(() {
        isLoading = false;
        message = "Profil alınamadı";
      });
    }
  }

  Future<void> updateProfile() async {
    try {
      await apiService.updateProfile(
        fullName: fullNameController.text,
        favoriteGenres: favoriteGenresController.text,
        languagePreference: languagePreferenceController.text,
        localOrForeign: localOrForeignController.text,
        watchingPurpose: watchingPurposeController.text,
      );

      setState(() {
        message = "Profil güncellendi";
      });
    } catch (e) {
      setState(() {
        message = "Profil güncellenemedi";
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Profile"), centerTitle: true),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(24),
              child: Column(
                children: [
                  TextField(
                    controller: fullNameController,
                    decoration: const InputDecoration(labelText: "Ad Soyad"),
                  ),
                  TextField(
                    controller: favoriteGenresController,
                    decoration: const InputDecoration(
                      labelText: "Favori Türler",
                      hintText: "Comedy|Action|Drama",
                    ),
                  ),
                  TextField(
                    controller: languagePreferenceController,
                    decoration: const InputDecoration(labelText: "Dil Tercihi"),
                  ),
                  TextField(
                    controller: localOrForeignController,
                    decoration: const InputDecoration(
                      labelText: "Yerli / Yabancı",
                    ),
                  ),
                  TextField(
                    controller: watchingPurposeController,
                    decoration: const InputDecoration(
                      labelText: "İzleme Amacı",
                    ),
                  ),
                  const SizedBox(height: 20),
                  ElevatedButton(
                    onPressed: updateProfile,
                    child: const Text("Profili Güncelle"),
                  ),
                  const SizedBox(height: 20),

                  ElevatedButton(
                    onPressed: () {
                      ApiService.token = null;
                      ApiService.userId = null;

                      Navigator.pushNamedAndRemoveUntil(
                        context,
                        '/',
                        (route) => false,
                      );
                    },
                    child: const Text("Çıkış Yap"),
                  ),
                  const SizedBox(height: 10),
                  Text(message),
                ],
              ),
            ),
    );
  }
}
