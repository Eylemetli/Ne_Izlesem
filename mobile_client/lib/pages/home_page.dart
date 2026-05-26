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

  String searchText = "";
  String selectedGenre = "";
  String selectedRating = "";

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

  Future<void> searchMovies() async {
    final data = await apiService.searchMovies(searchText);

    setState(() {
      movies = data;
    });
  }

  Future<void> filterMovies() async {
    final data = await apiService.filterMovies(selectedGenre, selectedRating);
    Future<void> resetMovies() async {
      setState(() {
        searchText = "";
        selectedGenre = "";
        selectedRating = "";
        isLoading = true;
      });

      await fetchMovies();
    }

    setState(() {
      movies = data;
    });
  }

  Future<void> resetMovies() async {
    setState(() {
      searchText = "";
      selectedGenre = "";
      selectedRating = "";
      isLoading = true;
    });

    await fetchMovies();
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
            : Column(
                children: [
                  TextField(
                    decoration: const InputDecoration(hintText: "Film ara..."),
                    onChanged: (value) {
                      searchText = value;
                    },
                  ),

                  const SizedBox(height: 10),

                  Row(
                    children: [
                      Expanded(
                        child: DropdownButtonFormField<String>(
                          value: selectedGenre.isEmpty ? null : selectedGenre,
                          decoration: const InputDecoration(labelText: "Tür"),
                          items: const [
                            DropdownMenuItem(
                              value: "Action",
                              child: Text("Action"),
                            ),
                            DropdownMenuItem(
                              value: "Comedy",
                              child: Text("Comedy"),
                            ),
                            DropdownMenuItem(
                              value: "Drama",
                              child: Text("Drama"),
                            ),
                          ],
                          onChanged: (value) {
                            selectedGenre = value ?? "";
                          },
                        ),
                      ),

                      const SizedBox(width: 10),

                      Expanded(
                        child: DropdownButtonFormField<String>(
                          value: selectedRating.isEmpty ? null : selectedRating,
                          decoration: const InputDecoration(
                            labelText: "Min Rating",
                          ),
                          items: const [
                            DropdownMenuItem(value: "5", child: Text("5+")),
                            DropdownMenuItem(value: "6", child: Text("6+")),
                            DropdownMenuItem(value: "7", child: Text("7+")),
                          ],
                          onChanged: (value) {
                            selectedRating = value ?? "";
                          },
                        ),
                      ),
                    ],
                  ),

                  const SizedBox(height: 10),

                  Row(
                    children: [
                      Expanded(
                        child: ElevatedButton(
                          onPressed: searchMovies,
                          child: const Text("Ara"),
                        ),
                      ),

                      const SizedBox(width: 10),

                      Expanded(
                        child: ElevatedButton(
                          onPressed: filterMovies,
                          child: const Text("Filtrele"),
                        ),
                      ),

                      const SizedBox(width: 10),

                      Expanded(
                        child: ElevatedButton(
                          onPressed: resetMovies,
                          child: const Text("Sıfırla"),
                        ),
                      ),
                    ],
                  ),

                  const SizedBox(height: 10),

                  Expanded(
                    child: GridView.builder(
                      itemCount: movies.length,
                      gridDelegate:
                          const SliverGridDelegateWithFixedCrossAxisCount(
                            crossAxisCount: 2,
                            childAspectRatio: 0.62,
                          ),
                      itemBuilder: (context, index) {
                        final movie = movies[index];

                        return MovieCard(movie: movie);
                      },
                    ),
                  ),
                ],
              ),
      ),
    );
  }
}
