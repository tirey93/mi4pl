Link do releasu: https://github.com/tirey93/mi4pl/releases/tag/converter
Program w pierwszej kolejności patrzy na ścieżkę w pliku config.json. Wystarczy wkleić swoją lokalną ścieżkę do pliku efmi.po w katalogu target w OmegaT.
jeśli go nie znajdzie to szuka pliku efmi.po w katalogu programu. 
jeśli go nie znajdzie to oczekuje parametru z nazwą pliku do przeprocesowania
Program posiada też opcjonalny parametr DestinationPath w config.json
jeśli jest ustawiony to plik script.tab zostanie zapisany zgodnie z parametrem
jeśli nie jest ustawiony to script.tab zostanie zapisany w miejscu uruchomienia programu

JAK UŻYWAĆ:
uruchom program wybierając po2tab_converter.exe
script.tab zostanie wygenerowany