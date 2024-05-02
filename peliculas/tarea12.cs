using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace P2.peliculas
{
    public class Tarea12
    {
        public class Reseña
        {
            [BsonElement("nombreUsuario")]
            public string? NombreUsuario
            {
                get; set;
            }
            [BsonElement("puntaje")]
            public int Puntaje
            {
                get; set;
            }
            [BsonElement("comentario")]
            public string? Comentario
            {
                get; set;
            }
        }
        public class Peliculas
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            [BsonElement("nombre")]
            public string? Nombre
            {
                get; set;
            }
            [BsonElement("genero")]
            public string? Genero
            {
                get; set;
            }
            [BsonElement("duracion")]
            public int Duracion
            {
                get; set;
            }
            [BsonElement("director")]
            public string? Director
            {
                get; set;
            }
            [BsonElement("sinopsis")]
            public string? Sinopsis
            {
                get; set;
            }
            [BsonElement("reseñas")]
            public List<Reseña> Reseñas
            {
                get; set;
            }
        }
        public static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017/");
            var cineDB = client.GetDatabase("cine");
            var peliculasCollection = cineDB.GetCollection<Peliculas>("peliculas");

            // Imprimir todas las peliculas con sus respectivas calificaciones
            var filter = Builders<Peliculas>.Filter.Empty;
            List<Peliculas> peliculasList = peliculasCollection.Find(filter).ToList();
            peliculasList.ForEach(pelicula => {
                Console.WriteLine("Película: " + pelicula.Nombre);
                Console.WriteLine("Puntajes:");
                pelicula.Reseñas.ForEach(reseña => {
                    Console.WriteLine(reseña.Puntaje);
                });
            });     
            
            // Realiza un interacción por el usuario donde puede elegir si quiere ver todo el listado de películas, 
            // o si quiere filtrar por nombre o por género. En la lista debe poder ver la duración y la sinopsis. 
            Console.WriteLine("¿Desea ver todas las películas o filtrar por nombre o género?");
            Console.WriteLine("1. Todas las películas");
            Console.WriteLine("2. Filtrar por nombre");
            Console.WriteLine("3. Filtrar por género");
            int opcion = Convert.ToInt32(Console.ReadLine());
            switch (opcion)
            {
                case 1:
                    peliculasList.ForEach(pelicula => {
                        Console.WriteLine("Película: " + pelicula.Nombre);
                        Console.WriteLine("Duración: " + pelicula.Duracion);
                        Console.WriteLine("Sinopsis: " + pelicula.Sinopsis);
                    });
                    break;
                case 2:
                    Console.WriteLine("Ingrese el nombre de la película:");
                    string nombre = Console.ReadLine();
                    var filterNombre = Builders<Peliculas>.Filter.Text(nombre);
                    List<Peliculas> peliculasNombreList = peliculasCollection.Find(filterNombre).ToList();
                    peliculasNombreList.ForEach(pelicula => {
                        Console.WriteLine("Película: " + pelicula.Nombre);
                        Console.WriteLine("Duración: " + pelicula.Duracion);
                        Console.WriteLine("Sinopsis: " + pelicula.Sinopsis);
                    });
                    break;
                case 3:
                    Console.WriteLine("Ingrese el género de la película:");
                    string genero = Console.ReadLine();
                    var filterGenero = Builders<Peliculas>.Filter.Regex("genero", new BsonRegularExpression(genero, "i"));                    
                    List<Peliculas> peliculasGeneroList = peliculasCollection.Find(filterGenero).ToList();
                    peliculasGeneroList.ForEach(pelicula => {
                        Console.WriteLine("Película: " + pelicula.Nombre);
                        Console.WriteLine("Duración: " + pelicula.Duracion);
                        Console.WriteLine("Sinopsis: " + pelicula.Sinopsis);
                        Console.WriteLine();
                    });
                    break;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            } 

            // El usuario debe añadir el nombre de la película, 
            // el género, la duración, la sinopsis y el director. 
            Console.WriteLine("¿Desea añadir una película?");
            Console.WriteLine("1. Sí");
            Console.WriteLine("2. No");
            int añadir = Convert.ToInt32(Console.ReadLine());
            if (añadir == 1)
            {
                Console.WriteLine("Ingrese el nombre de la película:");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingrese el género de la película:");
                string genero = Console.ReadLine();
                Console.WriteLine("Ingrese la duración de la película:");
                int duracion = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Ingrese la sinopsis de la película:");
                string sinopsis = Console.ReadLine();
                Console.WriteLine("Ingrese el director de la película:");
                string director = Console.ReadLine();
    
                Peliculas pelicula = new Peliculas
                {
                    Nombre = nombre,
                    Genero = genero,
                    Duracion = duracion,
                    Sinopsis = sinopsis,
                    Director = director,
                };
                peliculasCollection.InsertOne(pelicula);
            }

            // Sobre una película ya existente, el usuario puede agregar una nueva reseña con su nombre de usuario, puntaje y comentario.
            Console.WriteLine("¿Desea añadir una reseña a una película?");
            Console.WriteLine("1. Sí");
            Console.WriteLine("2. No");
            int añadirReseña = Convert.ToInt32(Console.ReadLine());
            if (añadirReseña == 1)
            {
                Console.WriteLine("Ingrese el nombre de la película:");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingrese su nombre de usuario:");
                string nombreUsuario = Console.ReadLine();
                Console.WriteLine("Ingrese el puntaje de la película:");
                int puntaje = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Ingrese el comentario de la película:");
                string comentario = Console.ReadLine();

                var filterNombre = Builders<Peliculas>.Filter.Eq("nombre", nombre);
                var pelicula = peliculasCollection.Find(filterNombre).FirstOrDefault();

                if (pelicula != null)
                {
                    // Si 'reseñas' no existe o es null, se iniciará un array vacío.
                    if (pelicula.Reseñas == null)
                    {
                        var updateInit = Builders<Peliculas>.Update.Set("reseñas", new BsonArray());
                        peliculasCollection.UpdateOne(filterNombre, updateInit);
                    }

                    // Se agrega la nueva reseña.
                    Reseña reseña = new Reseña
                    {
                        NombreUsuario = nombreUsuario,
                        Puntaje = puntaje,
                        Comentario = comentario,
                    };
                    var updatePush = Builders<Peliculas>.Update.Push("reseñas", reseña);
                    peliculasCollection.UpdateOne(filterNombre, updatePush);
                }
                else
                {
                    Console.WriteLine("Película no encontrada");
                }
            }
        }
    }
}
