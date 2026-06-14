# Reel Wars – Játékmechanikák és Architektúra

## Áttekintés

A játék egy kétjátékos, filmes témájú stratégiai játék, ahol a játékosok saját filmstúdiójukat irányítják. A cél az ellenfél stúdiójának elpusztítása különböző filmes karaktereket reprezentáló egységek segítségével.

A játék során a játékosok:
- Box Office bevételt gyűjtenek
- Popularity pontokat szereznek
- új műfajakat (Genre) nyitnak meg
- egyedi passzív képességeket szereznek

---

# Fő játékmenet

A játékos egységeket idéz meg a Clapperboard HUD segítségével.

Minden egység:
- pénzbe kerül
- rendelkezik életerővel
- sebzést okoz
- automatikusan mozog és harcol

Az egységek célja az ellenséges bázis elpusztítása.

---

# Gazdasági rendszer

A gazdasági rendszer alapját a `BoxOffice` osztály biztosítja.

Feladata:

- aktuális pénz nyilvántartása
- pénzköltés kezelése
- jutalmak jóváírása

A játékos csak akkor tud egységet létrehozni, ha rendelkezik elegendő pénzzel.

Kapcsolódó osztályok:

- `BoxOffice`
- `Player`
- `UnitType`
- `StatsConfig`

---

# Popularity rendszer

A fejlődés alapját a Popularity pontok jelentik.

A rendszer megvalósítása:

- `Popularity`
- `PopularityConfig`

Tárolt adatok:

- `CurrentPoints`
- `CurrentTier`

Popularity pontot a játékos ellenséges egységek megsemmisítésével szerez.

A pontszám elérésekor a játékos új műfajat választhat.

---

# Genre rendszer

A műfajokat a `GenreNode` ScriptableObject reprezentálja.

Egy műfaj tartalmaz:

- Melee egységet
- Ranged egységet
- Heavy egységet
- passzív képességet
- következő választható műfajakat

## Genre fa

```text
Black & White
├── Classic
│   ├── Drama
│   └── Comedy
└── Modern
    ├── Action
    └── Sci-Fi
```

---

# Genre-ek és egységeik

## Black & White

- Chaplin
- Cameraman
- King Kong

Passive:
- Partial refund on death

## Classic

- Columbo
- Zorro
- Frankenstein

Passive:
- Faster popularity gain

## Modern

- Indiana Jones
- Rambo
- Terminator

Passive:
- Bonus attack range

## Drama

- The Godfather
- Forrest Gump
- Titanic

Passive:
- Bonus attack speed

## Comedy

- Mr. Bean
- Jack Sparrow
- Shrek

Passive:
- Dodge chance

## Action

- Deadpool
- John Wick
- Hulk

Passive:
- Bonus damage

## Sci‑Fi

- Darth Vader
- Neo
- Thanos

Passive:
- Bonus movement speed

---

# Unit rendszer

Az egységek működését a `UnitBehaviour` osztály valósítja meg.

Az egységek statisztikáit a `UnitType` ScriptableObject tárolja.

Főbb attribútumok:

- Cost
- Damage
- MaxHealth
- MoveSpeed
- AttackRange
- AttackCooldown

Az egységek három kategóriába sorolhatók:

- Melee
- Ranged
- Heavy

Kapcsolódó osztályok:

- `UnitBehaviour`
- `UnitType`
- `UnitRole`

---

# Bázis rendszer

A játékos stúdióját a `BaseBehaviour` osztály reprezentálja.

Feladata:

- egységek létrehozása
- sérülések kezelése
- játékos referencia tárolása

A bázis vizuális elemei:

- stúdió épület
- neon felirat
- Popularity Meter
- Oscar szobor

---

# Popularity Meter

A Popularity nem egyszerű UI progress bar.

A stúdió mellett található egy külön világobjektum:

- függőleges mérőtorony
- Oscar szobor a tetején
- aktuális Popularity kijelzés

A mérő a játékos fejlődését reprezentálja.

---

# Clapperboard HUD

A felhasználói felület Unity UI Toolkit segítségével készült.

A Clapperboard panel tartalmazza:

- játékosnév
- aktuális Genre ikon
- Genre név
- Passive képesség
- Box Office érték
- három egység
- egység ikonok
- egység árak

Kapcsolódó osztályok:

- `ClapperboardSpawnerUI`
- `GenreChoiceUI`

---

# Genre választó felület

Szintlépéskor a játékos két új műfaj közül választhat.

A választó felület:

- rombusz alakú
- két választható Genre ikon
- billentyűzetes és egér alapú vezérlés

A választás után:

- módosul a `Player.CurrentGenre`
- frissül a HUD
- megváltoznak az egységek

---

# Input rendszer

A játék az új Unity Input Systemt használja.

Főbb funkciók:

- egység idézés
- műfajválasztás
- UI vezérlés

A bemeneteket a `GameInputActions` osztály kezeli.

---

# Konfigurációs rendszer

A játék konfigurációi ScriptableObject alapúak.

Főbb konfigurációk:

- `StatsConfig`
- `BaseStatsConfig`
- `RewardConfig`
- `PopularityConfig`
- `GenreConfig`
- `BoxOfficeConfig`

Ez lehetővé teszi a játék egyszerű balanszolását kódmódosítás nélkül.

---

# Fontosabb osztályok

```text
GameManager
Player
BaseBeaviour
BoxOffice
Popularity
GenreNode
UnitBeaviour
UnitType
ClapperboardSpawnerUI
GenreChoiceUI
```

---

# További fejlesztési lehetőségek

- Oscar Season események
- további Genre-ek
- további Passive képességek
- speciális egységek
- vizuális effektek
- hanghatások
- animációk
