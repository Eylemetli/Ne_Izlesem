import 'dart:convert';
import 'package:http/http.dart' as http;

class ApiService {
  static const String baseUrl = "http://10.0.2.2:5148/api";

  Future<List<dynamic>> getMovies() async {
    final response = await http.get(Uri.parse("$baseUrl/Movie"));

    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception("Filmler alınamadı");
    }
  }

  Future<List<dynamic>> getRecommendations() async {
    final response = await http.get(
      Uri.parse("$baseUrl/Recommendation/me"),
      headers: {"Authorization": "Bearer $token"},
    );

    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception("Recommendation alınamadı");
    }
  }

  static String? token;
  static int? userId;

  Future<void> login(String email, String password) async {
    final response = await http.post(
      Uri.parse("$baseUrl/Auth/login"),
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({"email": email, "password": password}),
    );

    if (response.statusCode == 200) {
      final data = jsonDecode(response.body);
      token = data["token"];
      userId = data["userId"];
    } else {
      throw Exception("Login failed");
    }
  }

  Future<void> addToWatchlist(int movieId) async {
    final response = await http.post(
      Uri.parse("$baseUrl/Watchlist?userId=$userId&movieId=$movieId"),
      headers: {"Authorization": "Bearer $token"},
    );

    if (response.statusCode != 200) {
      throw Exception("Watchlist eklenemedi");
    }
  }

  Future<List<dynamic>> getWatchlist() async {
    final response = await http.get(
      Uri.parse("$baseUrl/Watchlist/$userId"),
      headers: {"Authorization": "Bearer $token"},
    );

    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception("Watchlist alınamadı");
    }
  }

  Future<Map<String, dynamic>> getProfile() async {
    final response = await http.get(
      Uri.parse("$baseUrl/Profile/me"),
      headers: {"Authorization": "Bearer $token"},
    );

    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception("Profil alınamadı");
    }
  }

  Future<void> updateProfile({
    required String fullName,
    required String favoriteGenres,
    required String languagePreference,
    required String localOrForeign,
    required String watchingPurpose,
  }) async {
    final response = await http.put(
      Uri.parse("$baseUrl/Profile/me"),
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer $token",
      },
      body: jsonEncode({
        "fullName": fullName,
        "favoriteGenres": favoriteGenres,
        "languagePreference": languagePreference,
        "localOrForeign": localOrForeign,
        "watchingPurpose": watchingPurpose,
      }),
    );

    if (response.statusCode != 200) {
      throw Exception("Profil güncellenemedi");
    }
  }

  Future<List<dynamic>> searchMovies(String query) async {
    if (query.trim().isEmpty) {
      return [];
    }

    final encodedQuery = Uri.encodeComponent(query.trim());

    final response = await http.get(
      Uri.parse("$baseUrl/Movie/search?query=$encodedQuery"),
    );

    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      print("SEARCH STATUS: ${response.statusCode}");
      print("SEARCH BODY: ${response.body}");
      return [];
    }
  }

  Future<List<dynamic>> filterMovies(String genre, String minRating) async {
    final response = await http.get(
      Uri.parse("$baseUrl/Movie/filter?genre=$genre&minRating=$minRating"),
    );

    if (response.statusCode == 200) {
      return jsonDecode(response.body);
    } else {
      throw Exception("Filtreleme başarısız");
    }
  }

  Future<void> rateMovie(int movieId, double rating) async {
    final response = await http.post(
      Uri.parse("$baseUrl/Rating?movieId=$movieId&rating=$rating"),
      headers: {"Authorization": "Bearer $token"},
    );

    if (response.statusCode != 200) {
      throw Exception("Puan verilemedi");
    }
  }

  Future<void> register({
    required String fullName,
    required String email,
    required String password,
  }) async {
    final response = await http.post(
      Uri.parse("$baseUrl/Auth/register"),
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({
        "fullName": fullName,
        "email": email,
        "password": password,
      }),
    );

    if (response.statusCode != 200) {
      throw Exception("Kayıt başarısız");
    }
  }
}
