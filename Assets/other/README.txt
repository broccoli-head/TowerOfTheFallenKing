Dodawanie przeciwnikow:
- utworzyc GameObject przeciwnika
- dodac do niego Collider2d
- dodac skrypt Enemy.cs
- w skrypcie Enemy ustawc zachowanie i statystyki przeciwnika

Dodawanie mikstur:
- w obiekcie Inventory -> skrypt inventory -> lista Potions
- dodac element listy
- Uzupelnic statystyki, nazwe i opis
- zmienna potion:
	* utworzyc prefab z grafika butelki
	* dodac collider2d 
	* dodac skrypt PotionObject.cs i jako name wpisac nazwe mikstury
- zmienna potion effect:
	* utworzyc prefab z grafika efektu (ogien, kolce itp.)
	* dodac collider2d (trigger)
	* dodac skrypt PotionEffect.cs i jako name wpisac nazwe mikstury

Dodawanie zasobow:
- w obiekcie Inventory -> skrypt inventory -> lista Resources
- dodac elemet listy
- uzupelnic wlasciwosci, sprite odpowiada za grafike wyswietlana w UI

Dodawanie Mikstur i Zasobow do inventory gracza:
- w obiekcie Inventory -> skrypt inventory -> lista PlayerPotions lub PlayerResources
- dodac element listy, PotionName/ResourceName - nazwa itemu, count - ilosc posiadanych przez gracza przedmiotow tego typu

Dodawanie Receptur:
- w obiekcie Inventory -> skrypt inventory -> lista Recipes
- dodac elemet listy
- ItemName - nazwa itemu ktory jest rezultatem craftowania
- listy Fillers i components - listy skladnikow i wypelniaczy do mikstury:
	- wybierasz czy skladnik jest zasobem z danym atrybutem czy konkretna mikstura
	  (Zasoby maja wlasciwosci, mikstury nie, przy craftowaniu uzywana jest wlasciwosc zasobu lub name potki)
	- wpisujesz Name mikstury lub wybierasz wlasciwosc
	- wybierasz temperature
- Wlasciwosci (Attributes) i temperatury mozesz dodawac w Item.cs

Dodawanie interakcji:
- nie sprawdzone, na razie nie ruszaj