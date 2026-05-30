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

  String languagePreference = "";
  String localOrForeign = "";
  String watchingPurpose = "";
  List<String> selectedGenres = [];
  List<String> genreList = [];

  String message = "";
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    fetchGenres();
    fetchProfile();
  }

  Future<void> fetchGenres() async {
    final data = await apiService.getGenres();
    setState(() {
      genreList = data;
    });
  }

  Future<void> fetchProfile() async {
    try {
      final data = await apiService.getProfile();

      fullNameController.text = data["fullName"] ?? "";
      languagePreference = data["languagePreference"] ?? "";
      localOrForeign = data["localOrForeign"] ?? "";
      watchingPurpose = data["watchingPurpose"] ?? "";

      if (data["favoriteGenres"] != null && data["favoriteGenres"] != "") {
        selectedGenres = (data["favoriteGenres"] as String).split("|");
      }

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
        favoriteGenres: selectedGenres.join("|"),
        languagePreference: languagePreference,
        localOrForeign: localOrForeign,
        watchingPurpose: watchingPurpose,
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

  void toggleGenre(String genre) {
    setState(() {
      if (selectedGenres.contains(genre)) {
        selectedGenres.remove(genre);
      } else {
        selectedGenres.add(genre);
      }
    });
  }

  Widget buildDropdown(
    String label,
    String value,
    List<String> options,
    Function(String?) onChanged,
  ) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: DropdownButtonFormField<String>(
        value: value.isEmpty ? null : value,
        decoration: InputDecoration(labelText: label),
        items: options
            .map((o) => DropdownMenuItem(value: o, child: Text(o)))
            .toList(),
        onChanged: onChanged,
      ),
    );
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
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  TextField(
                    controller: fullNameController,
                    decoration: const InputDecoration(labelText: "Ad Soyad"),
                  ),

                  const SizedBox(height: 16),
                  const Text(
                    "Favori Türler",
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: genreList.map((genre) {
                      final isSelected = selectedGenres.contains(genre);
                      return GestureDetector(
                        onTap: () => toggleGenre(genre),
                        child: Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 6,
                          ),
                          decoration: BoxDecoration(
                            color: isSelected ? Colors.red : Colors.grey[800],
                            borderRadius: BorderRadius.circular(20),
                          ),
                          child: Text(
                            genre,
                            style: TextStyle(
                              color: Colors.white,
                              fontWeight: isSelected
                                  ? FontWeight.bold
                                  : FontWeight.normal,
                            ),
                          ),
                        ),
                      );
                    }).toList(),
                  ),

                  const SizedBox(height: 8),

                  buildDropdown(
                    "Dil Tercihi",
                    languagePreference,
                    ["English", "Turkish", "Other"],
                    (val) => setState(() => languagePreference = val ?? ""),
                  ),

                  buildDropdown(
                    "Yerli / Yabancı",
                    localOrForeign,
                    ["Local", "Foreign", "Both"],
                    (val) => setState(() => localOrForeign = val ?? ""),
                  ),

                  buildDropdown(
                    "İzleme Amacı",
                    watchingPurpose,
                    ["Entertainment", "Learning", "Relaxation", "Other"],
                    (val) => setState(() => watchingPurpose = val ?? ""),
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
