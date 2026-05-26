import 'package:flutter/material.dart';
import '../services/api_service.dart';
import '../widgets/movie_card.dart';

class WatchlistPage extends StatefulWidget {
  const WatchlistPage({super.key});

  @override
  State<WatchlistPage> createState() => _WatchlistPageState();
}

class _WatchlistPageState extends State<WatchlistPage> {
  final ApiService apiService = ApiService();

  List movies = [];
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    fetchWatchlist();
  }

  Future<void> fetchWatchlist() async {
    try {
      final data = await apiService.getWatchlist();

      setState(() {
        movies = data;
        isLoading = false;
      });
    } catch (e) {
      setState(() {
        isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("Watchlist"), centerTitle: true),
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
