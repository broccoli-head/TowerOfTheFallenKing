Dodawanie przeciwników:
- utworzyæ GameObject przeciwnika
- dodaæ do niego Collider2d
- dodaæ skrypt Enemy.cs
- w skrypcie Enemy ustawæ zachowanie i statystyki przeciwnika

Dodawanie mikstur:
- w obiekcie Inventory -> skrypt inventory -> lista Potions
- dodaæ element listy
- Uzupe³niæ statystyki, nazwê i opis
- zmienna potion:
	* utworzyc prefab z grafik¹ butelki
	* dodaæ collider2d 
	* dodaæ skrypt PotionObject.cs i jako name wpisaæ nazwê mikstury
- zmienna potion effect:
	* utworzyæ prefab z grafika efektu (ogieñ, kolce itp.)
	* dodaæ collider2d (trigger)
	* dodaæ skrypt PotionEffect.cs i jako name wpisaæ nazwê mikstury

Dodawanie zasobów:
- w obiekcie Inventory -> skrypt inventory -> lista Resources
- dodaæ elemêt listy
- uzupe³niæ w³aœciwoœci, sprite odpowiada za grafikê wyœwietlan¹ w UI

Dodawanie Mikstur i Zasobów do inventory gracza:
- w obiekcie Inventory -> skrypt inventory -> lista PlayerPotions lub PlayerResources
- dodaæ element listy, PotionName/ResourceName - nazwa itemu, count - iloœæ posiadanych przez gracza przedmiotów tego typu

Dodawanie Receptur:
- w obiekcie Inventory -> skrypt inventory -> lista Recipes
- dodaæ elemêt listy
- ItemName - nazwa itemu który jest rezultatem craftowania
- listy Fillers i components - listy skladników i wypelniaczy do mikstury:
	- wybierasz czy sk³adnik jest zasobem z danym atrybutem czy konkretn¹ mikstur¹
	  (Zasoby maj¹ w³asciwosci, mikstury nie, przy craftowaniu u¿ywana jest wlasciwosc zasobu lub name potki)
	- wpisujesz Name mikstury lub wybierasz wlasciwosc
	- wybierasz temperature
- Wlasciwosci (Attributes) i temperatury mozesz dodawaæ w Item.cs

Dodawanie interakcji:
- nie sprawdzone, na razie nie ruszaj