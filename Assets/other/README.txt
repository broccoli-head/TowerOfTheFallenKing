Dodawanie przeciwnik�w:
- utworzy� GameObject przeciwnika
- doda� do niego Collider2d
- doda� skrypt Enemy.cs
- w skrypcie Enemy ustaw� zachowanie i statystyki przeciwnika

Dodawanie mikstur:
- w obiekcie Inventory -> skrypt inventory -> lista Potions
- doda� element listy
- Uzupe�ni� statystyki, nazw� i opis
- zmienna potion:
	* utworzyc prefab z grafik� butelki
	* doda� collider2d 
	* doda� skrypt PotionObject.cs i jako name wpisa� nazw� mikstury
- zmienna potion effect:
	* utworzy� prefab z grafika efektu (ogie�, kolce itp.)
	* doda� collider2d (trigger)
	* doda� skrypt PotionEffect.cs i jako name wpisa� nazw� mikstury

Dodawanie zasob�w:
- w obiekcie Inventory -> skrypt inventory -> lista Resources
- doda� elem�t listy
- uzupe�ni� w�a�ciwo�ci, sprite odpowiada za grafik� wy�wietlan� w UI

Dodawanie Mikstur i Zasob�w do inventory gracza:
- w obiekcie Inventory -> skrypt inventory -> lista PlayerPotions lub PlayerResources
- doda� element listy, PotionName/ResourceName - nazwa itemu, count - ilo�� posiadanych przez gracza przedmiot�w tego typu

Dodawanie Receptur:
- w obiekcie Inventory -> skrypt inventory -> lista Recipes
- doda� elem�t listy
- ItemName - nazwa itemu kt�ry jest rezultatem craftowania
- listy Fillers i components - listy skladnik�w i wypelniaczy do mikstury:
	- wybierasz czy sk�adnik jest zasobem z danym atrybutem czy konkretn� mikstur�
	  (Zasoby maj� w�asciwosci, mikstury nie, przy craftowaniu u�ywana jest wlasciwosc zasobu lub name potki)
	- wpisujesz Name mikstury lub wybierasz wlasciwosc
	- wybierasz temperature
- Wlasciwosci (Attributes) i temperatury mozesz dodawa� w Item.cs

Dodawanie interakcji:
- nie sprawdzone, na razie nie ruszaj