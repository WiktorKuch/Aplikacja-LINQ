
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Csvhelper.Configuration;
using Aplikacja_LINQ;

namespace AplikacjaLINQ 
{ 
    class Program   // linq bazuje na rozszerzeniu interfejsu IEnumerable 
    {
        static void Main(string[] args)
        {
            string csvPath = @"D:\Dataset\googleplaystore1.csv"; // deklaracja sciezki do pliku csv
            var googleApps = LoadGoogleAps(csvPath);  // lista obiektów typu googleApp

            //Display(googleApps);
            //GetData(googleApps);
            //ProjectData(googleApps);
            //DivideData(googleApps);
            //OrderData(googleApps);
            //DataSetOperation(googleApps);
            //DataVerification(googleApps);
            //GroupData(googleApps);
            //GroupDataOperations(googleApps);


            
        }

        static void GroupDataOperations(IEnumerable<GoogleApp> googleApps)
        {
            var categoryGroup = googleApps
                .GroupBy(g => g.Category)
                .Where(g => g.Min(a => a.Reviews) >= 10);

            foreach(var group in categoryGroup)
            {
                var averageReviews = group.Average(g => g.Reviews);
                var minReviews = group.Min(g=>g.Reviews);
                var maxReviews = group.Max(g => g.Reviews);
                var reviewsCount = group.Sum(g => g.Reviews);
                var allAppsFromGroupHaveRatingOfThree = group.All(g => g.Rating > 3);

                Console.WriteLine($"Category: {group.Key}");
                Console.WriteLine($"averageReviews: {averageReviews}");
                Console.WriteLine($"minReviews: {minReviews}");
                Console.WriteLine($"maxReviews: {maxReviews}");
                Console.WriteLine($"reviewsCount: {reviewsCount}");
                Console.WriteLine($"llAppsFromGroupHaveRatingOfThree : {allAppsFromGroupHaveRatingOfThree}");


            }
                
        }

        static void GroupData(IEnumerable<GoogleApp> googleApps)
        {
            var categoryGroup = googleApps.GroupBy(e=> new { e.Category, e.Type });

           foreach(var group in categoryGroup)
            {
                var key = group.Key;

                var apps = group.ToList();
                Console.WriteLine($"Displaing elements for group {group.Key.Category} , {group.Key.Type}");
                Display(apps);
            }
        }

        static void DataVerification(IEnumerable<GoogleApp> googleApps)
        {
            var allOperatorResult = googleApps.Where(a => a.Category == Category.WEATHER).All(a => a.Reviews > 20);

            Console.WriteLine($"allOperatorResult {allOperatorResult}");

            var anyOperatorResult = googleApps.Where(a=>a.Category== Category.WEATHER).Any(a => a.Reviews > 3_000_000);

            Console.WriteLine($"anyOperatorResult {anyOperatorResult}");

        }

        static void DataSetOperation(IEnumerable<GoogleApp> googleApps)
        {
            var paidAppsCategories = googleApps.Where(a => a.Type == Type.Paid).Select(a => a.Category).Distinct(); // brak duplikatów

            Console.WriteLine($"Paid apps categories : {string.Join(", ", paidAppsCategories)}");

            var setA = googleApps.Where(a=>a.Rating>4 && a.Type == Type.Paid && a.Reviews >1000 );
            var setB = googleApps.Where(a => a.Name.Contains("Pro") && a.Rating > 4 && a.Reviews > 10000);

            var appsUnion = setA.Union(setB);  // tylko te typy które sie pokrywają
            Console.WriteLine("Apps union");
            Display(appsUnion);

            var appsIntersect = setA.Intersect(setB); // typy sie pokrywaja 
            Console.WriteLine("Apps intersect");   // zarówno wystepuja w z.A jak i w z.B
            Display(appsIntersect);

            var appsExcept = setA.Except(setB);  //except - na zbiorze A wywołamy op. Except  dla z.B to rez. te elementy ktore sa w z.A i jednoczesnie nie wys w z.B
            Console.WriteLine("Apps except");
            Display(appsExcept);


        }

        static void OrderData(IEnumerable<GoogleApp> googleApps) // sortowanie-zwrócenie nowej kolekcji na podstawie bazowej kolecji której elementy beda w konkretnej kolejnosci
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 5 && app.Category == Category.BEAUTY);
            

            var sortedResults = highRatedBeautyApps.OrderByDescending(app => app.Rating).ThenByDescending(app=>app.Name).Take(5);   // lambda wskazuje po jakim polu bedzie sortowanie a potem po drugim , tylko 5 ele
            Display(sortedResults);                                                                     
        }

        static void DivideData(IEnumerable<GoogleApp> googleApps) // dzielenie danych - zwrocenie konkretnej liczby danych lub pominiecie konkretnej liczby wierszy 
        {
            var highRatedBeautyApps = googleApps.Where(app => app.Rating > 5 && app.Category == Category.BEAUTY);
            Display(highRatedBeautyApps);
            

            var first5HighRatedBeautyApps = highRatedBeautyApps.TakeWhile(app => app.Reviews>1000);  //liczba wyników która chcemy zwrocic tak długo jak warunek bedzie prawdziwy
            Display(first5HighRatedBeautyApps);


            var skippedResults = highRatedBeautyApps.SkipWhile(app => app.Reviews > 1000);   //  pierwszy element który spełnia warunek a potem reszta  
            Console.WriteLine("Skipped result:");
            Display(skippedResults);
            
        }

        static void ProjectData(IEnumerable<GoogleApp> googleApps) // zmiana jednego zbioru danych na drugi na podstawie tego pierwszego zbioru danych
        {
            var highRatedBeautyApps = googleApps.Where(app=> app.Rating > 5 && app.Category == Category.BEAUTY);
            var highRatedBeutyAppsNames = highRatedBeautyApps.Select(app => app.Name); // wybranie przy pomocy selektora


            var dtos = highRatedBeautyApps.Select(app=> new GoogleAppDto() // dla kazdego ele typu googleapps naszej kolekcji chcemy zwrocic nowy obiekt klasy googledto w ktorym sprecyzowalismy ze, etc
            { 
                Reviews=app.Reviews,
                Name = app.Name
            }); 

            var anoymousDtos = highRatedBeautyApps.Select(app => new  
            { 
                Reviews = app.Reviews,
                Name = app.Name ,
                Category=app.Category
            });

            foreach (var dto in dtos/*lub anonymousDtos*/)   //zwracanie kolekcji typu anonimowego - typ dla ktorego nie mamy okreslonej klasy
            {
                Console.WriteLine($"{dto.Name}: {dto.Reviews}");
            }


            var genres = highRatedBeautyApps.SelectMany(app => app.Genres); // lista wszystkich gatunków tych aplikacji

            Console.WriteLine(string.Join(":", genres));

            
        }

        static void GetData(IEnumerable<GoogleApp> googleApps)
        {                          // ocenianie aplikacji where jako filtr którego parametrem jest funkcja - konk.lambda
           
            var highRatedApps = googleApps.Where((/*GoogleApp*/ app)=>app.Rating>5);  // ocena 
            var highRatedBeautyApps = googleApps.Where((/*GoogleApp*/ app) => app.Rating > 5 && app.Category ==Category.BEAUTY);  // ocena i kategoria beuty
            
            Display(highRatedBeautyApps);

            var firstHighRatedBeautyApp = highRatedBeautyApps.SingleOrDefault(app=> app.Reviews < 300);// First(),Single(),Last() wyrzuci wyjatki,wyswietli nie 1 lecz kilka ele,z liczba pobran < 300
            Console.WriteLine("firstHighRatedBeautyApp");
            Console.WriteLine(firstHighRatedBeautyApp);

        }

        static void Display(IEnumerable<GoogleApp> googleApps)  // przyjmuje kolekcje typu googleapp 
        {
            foreach(var googleApp in googleApps)  // wypisujemy do konsoli kazdy element
            {
                Console.WriteLine(googleApp);
            }
        }

        static void Display(GoogleApp googleApp) // to samo ale  dla jednego konkretnego elementu 
        {
            Console.WriteLine(googleApp);
        }

        static List<GoogleApp> LoadGoogleAps(string csvPath)
        {
            using(var reader = new StreamReader(csvPath))
            using(var csv = new CsvReader(reader, CulturInfo.InvariantCulture))   //za pomoca klasy CsvReader czytamy wszystkie rekordy z pliku csv i zwracamy
            {
                csv.Context.RegisterClassMap<GoogleAppMap>().ToList();
                var records = csv.GetRecords<GoogleApp>().ToList();
                return records;



            }
        }
    }






}
