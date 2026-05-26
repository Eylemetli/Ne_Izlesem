import 'package:flutter/material.dart';
import '../widgets/movie_card.dart';
import '../services/api_service.dart';
import 'watchlist_page.dart';
import 'profile_page.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  final ApiService apiService = ApiService();

  List movies = [];

  bool isLoading = true;

  @override
  void initState() {
    super.initState();

    fetchMovies();
  }

  Future<void> fetchMovies() async {
    try {
      final data = await apiService.getRecommendations();

      setState(() {
        movies = data;
        isLoading = false;
      });
    } catch (e) {
      print(e);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Movie Recommendation"),
        centerTitle: true,
      ),
      body: Padding(
        padding: const EdgeInsets.all(12),
        child: isLoading
            ? const Center(child: CircularProgressIndicator())
            : GridView.builder(
                itemCount: movies.length,
                gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: 2,
                  childAspectRatio: 0.62,
                ),
                itemBuilder: (context, index) {
                  final movie = movies[index];

                  return MovieCard(movie: movie);
                },
              ),
      ),
    );
  }
}
