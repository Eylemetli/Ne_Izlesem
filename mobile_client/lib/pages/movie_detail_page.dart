import 'package:flutter/material.dart';
import '../services/api_service.dart';

class MovieDetailPage extends StatelessWidget {
  final Map movie;

  const MovieDetailPage({super.key, required this.movie});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(movie["title"] ?? "Film Detayı")),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (movie["posterUrl"] != null)
              Center(
                child: Image.network(
                  movie["posterUrl"],
                  height: 420,
                  fit: BoxFit.cover,
                ),
              ),

            const SizedBox(height: 20),

            Text(
              movie["title"] ?? "",
              style: const TextStyle(fontSize: 28, fontWeight: FontWeight.bold),
            ),

            const SizedBox(height: 10),

            Text("⭐ ${movie["voteAverage"] ?? ""}"),
            const SizedBox(height: 20),

            ElevatedButton(
              onPressed: () async {
                try {
                  await ApiService().addToWatchlist(movie["id"]);

                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(content: Text("Watchliste eklendi")),
                  );
                } catch (e) {
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(content: Text("Film zaten ekli olabilir")),
                  );
                }
              },
              child: const Text("Watchliste Ekle"),
            ),
            const SizedBox(height: 20),

            Row(
              children: [
                for (int i = 1; i <= 5; i++)
                  IconButton(
                    onPressed: () async {
                      try {
                        await ApiService().rateMovie(movie["id"], i.toDouble());

                        ScaffoldMessenger.of(context).showSnackBar(
                          SnackBar(content: Text("$i puan verildi")),
                        );
                      } catch (e) {
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(content: Text("Puan verilemedi")),
                        );
                      }
                    },
                    icon: const Icon(Icons.star),
                    color: Colors.amber,
                  ),
              ],
            ),

            const SizedBox(height: 20),

            Text(movie["overview"] ?? ""),
          ],
        ),
      ),
    );
  }
}
