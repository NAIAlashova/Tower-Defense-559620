# Лабораторная 3 --- Прототип Tower Defense

## О проекте

Небольшая 3D-игра на Godot 4 с C# с участием игрока, системой ресурсов, ядром базы и разнообразными способами нанесения урона. 
Игра спроектирована с помощью композиции, где каждая функция вынесена в отдельный компонент.

## Структура проекта

``` text
lab3-559620/
├── Components/
│   ├── Bullet.cs
│   ├── Core.cs
│   ├── DamagingComponent.cs
│   ├── ExplosiveComponent.cs
│   ├── HealthComponent.cs
│   ├── InventaryComponent.cs
│   ├── MoveComponent.cs
│   ├── Player.cs
│   ├── PlayersShootingComponent.cs
│   ├── ShootingComponent.cs
│   ├── SlowingComponent.cs
│   ├── TrackerComponent.cs
│   └── VisibleComponent.cs
├── Scenes/
│   ├── BaseScene.tscn
│   ├── Core.tscn
│   ├── DebrisScene.tscn
│   ├── bullet.tscn
│   ├── enemie_tower(t1).tscn
│   ├── enemy_tower(t2).tscn
│   ├── enemy_tower(t3).tscn
│   ├── player.tscn
│   ├── testTower.tscn
│   ├── time_bullet.tscn
│   ├── tower(t1).tscn
│   ├── tower(t2).tscn
│   └── tower(t3).tscn
├── project.godot
└── README.md
```

-   `Components/` --- C#-скрипты игрока, ядра базы и игровых компонентов:
    движение, обнаружение целей, стрельба, здоровье, урон, замедление,
    взрывы, учёт ресурсов и обработка уничтожения объектов.
-   `Scenes/BaseScene.tscn` --- основная сцена уровня.
-   `Scenes/player.tscn` и `Scenes/Core.tscn` --- сцены игрока и ядра базы.
-   `Scenes/tower(t1).tscn`, `Scenes/tower(t2).tscn`, `Scenes/tower(t3).tscn`
    --- союзные башни: обычная, замедляющая и взрывная соответственно.
-   `Scenes/enemie_tower(t1).tscn`, `Scenes/enemy_tower(t2).tscn`,
    `Scenes/enemy_tower(t3).tscn` --- вражеские варианты трёх типов башен
    с компонентом движения.
-   `Scenes/bullet.tscn` и `Scenes/time_bullet.tscn` --- обычный и замедляющий
    снаряды на основе `Area3D`.
-   `Scenes/DebrisScene.tscn` --- физический обломок для эффекта взрыва.
-   `Scenes/testTower.tscn` --- вспомогательная сцена макета башни для режима строительства башен.


## Основные компоненты

### `VisibleComponent`

Наследуется от `Area3D`. Получает родительскую башню и ищет среди тел,
пересекающих область обнаружения, объекты противоположной команды. Если
найдено несколько целей, выбирает ближайшую. Метод `GetTarget()`
возвращает выбранную цель для компонентов движения и стрельбы.

### `ShootingComponent`

Отвечает за автоматическую стрельбу. Получает родительскую башню и
ссылку на `VisibleComponent`. Когда цель обнаружена и перезарядка
завершена, создаёт пулю из `Scenes/bullet.tscn`, задаёт ей группу, по
которой нужно наносить урон, и направляет её в сторону цели. Для башни
из группы `allies` пуля нацелена на `enemies`, а для башни из `enemies`
--- на `allies`.

### `PlayersShootingComponent`

Наследуется от `ShootingComponent`. Отвечает за стрельбу игрока при
нажатии действия `shoot`. Создаёт пулю из `BulletScene`, задаёт ей
группу противника и поворачивает в ту же сторону, куда смотрит игрок.
Для игрока устанавливает урон пули равным 10.

### `Bullet`

Наследуется от `Area3D`. Двигается вперёд по направлению своей локальной
оси `-Z`. При сигнале `body_entered` запускает метод `Hit` дочернего узла DamagingComponent.

### `DamagingComponent`

В методе `Hit` проверяет, входит ли столкнувшееся
тело в группу, указанную в `damagingGroup`. Если входит, ищет среди
непосредственных дочерних узлов тела компонент `HealthComponent`,
наносит ему урон и удаляет пулю. 

### `SlowingComponent`

Наследуется от `DamagingComponent`. В методе `Damage()` проверяет тип
цели. Если цель --- игрок, вызывает его метод `Slowdown()`. Для остальных
объектов ищет среди дочерних узлов `MoveComponent` и вызывает
`Slowdown()` у него. Вместо нанесения урона замедляет движение цели.

### `ExplosiveComponent`

Наследуется от `DamagingComponent`. Получает родительскую башню и
устанавливает урон равным 70. При сигнале `body_entered` проверяет группу
столкнувшегося тела и вызывает `Hit()`. В методе `Damage()` находит
`HealthComponent` цели и наносит ему урон. Для эффекта взрыва создаёт
20 обломков из `Scenes/DebrisScene.tscn` и задаёт им случайные импульсы
движения и вращения.

### `HealthComponent`

Хранит максимальное и текущее здоровье объекта. При запуске
устанавливает текущее здоровье равным максимальному. Метод
`TakeDamage()` уменьшает здоровье; если оно падает ниже 1, компонент
вызывает `Die()`, который выводит сообщение и удаляет родительскую башню
из сцены.

### `MoveComponent`

Отвечает за движение башни к выбранной цели. Получает цель через
`VisibleComponent` и перемещает родительский `CharacterBody3D` в её
сторону, игнорируя разницу по высоте. Если расстояние до цели не
превышает `StopRadius`, движение прекращается.

### `InventaryComponent`

Наследуется от `Node`. Хранит текущее количество ресурсов, стоимость
постройки башен и награду за их уничтожение. При запуске устанавливает
начальное количество ресурсов и обновляет текст в интерфейсе. Метод
`CanBuildTower()` проверяет, хватает ли ресурсов на выбранную башню,
а `DeleteResources()` списывает её стоимость. Метод `AddResources()`
определяет тип уничтоженной башни по её компонентам и добавляет награду.
После изменения ресурсов обновляет надпись через `UpdateCurrencyLabel()`.

### `TrackerComponent`

Наследуется от `Node`. Получает ссылку на `InventaryComponent` и
обрабатывает уничтожение объектов в методе `DieObject()`. Если уничтожен
игрок или ядро базы, удаляет текущую сцену. Если уничтожена башня из
группы `enemies`, вызывает `AddResources()` для начисления награды.

### `Player`

Наследуется от `CharacterBody3D`. Отвечает за движение игрока и поворот
в сторону мыши. Получает `HealthComponent` и обновляет надпись здоровья
при сигнале `HealthChanged`. При действии `pause` переключает режим
строительства и вызывает `Pause()` у компонентов движения и стрельбы.
В режиме строительства показывает макет башни и позволяет выбрать один
из трёх типов. При выходе проверяет ресурсы через `InventaryComponent`,
создаёт выбранную башню и списывает её стоимость. Если ресурсов не
хватает, показывает сообщение. Метод `Slowdown()` включает замедление:
скорость движения уменьшается в три раза на 20 секунд.

### `Core`

Наследуется от `CharacterBody3D`. Обозначает ядро базы и используется в `TrackerComponent` для проверки типа
уничтоженного объекта: при уничтожении ядра текущая сцена удаляется.

## Как компоненты взаимодействуют

1.  `VisibleComponent` обнаруживает башни противоположной команды и
    возвращает ближайшую цель.
2.  `ShootingComponent` использует эту цель, создаёт пулю и задаёт ей
    группу противника.
3.  `HealthComponent` уменьшает здоровье. Когда оно заканчивается,
    родительская башня удаляется.
4.  `MoveComponent` использует обнаруженную цель, чтобы приблизить
    вражескую башню к ней.
5.  `PlayersShootingComponent` отвечает за стрельбу игрока: при действии
    `shoot`  создаёт пулю и задаёт ей группу противника и урон его компоненту нанесения урона.
6.  `DamagingComponent` обнаруживает столкновение через `body_entered`, проверяет
    группу цели. В случае подходящей группы вызывает `Hit` и далее `Damege`,
    который находит `HealthComponent` цели.
    После попадания в любой объект, не являющийся родительским, узел удаляется.
7.  `SlowingComponent` заменяет обычный урон замедлением. 
    Имеет переписанный `Damage()`, который вызывает
    `Player.Slowdown()` или `MoveComponent.Slowdown()` у цели.
8.  `ExplosiveComponent` отвечает за взрывную атаку. 
    Имеет переписанный `Damage()`, который как и раньше обращается к
    `HealthComponent.TakeDamage()`, но теперь создаёт обломки, имитьрующие взрыв.
9.  `InventaryComponent` учитывает ресурсы и экономиру. `Player` вызывает
    `CanBuildTower()` для проверки, сможет ли он построить башню, и 
    `DeleteResources()` после неё для учитывания стоимости башню.
    При уничтожении врага `TrackerComponent` вызывает `AddResources()`,
    который определяет награду через и обновляет количесство ресурсов.
10. `TrackerComponent` обрабатывает уничтожение объектов. Когда здоровье
    заканчивается, `HealthComponent.Die()` передаёт объект в `DieObject()`.
    Для вражеской башни он вызывает `InventaryComponent.AddResources()`,
    а для игрока или ядра базы удаляет текущую сцену, что означает окончание игры.
11. `Player` управляет движением игрока и строительством. При переключении режима на строительство
    вызывает `MoveComponent.Pause()` и `ShootingComponent.Pause()`.
    Также является связующим звеном между `HealthComponent` игрока и `HealthLabel`.
12. `Core` обозначает ядро базы. 
    Его здоровье хранится в дочернем `HealthComponent`.

Компоненты подключены к дочерним узлам через сцены Godot:

-   `Scenes/tower(t1).tscn` --- `HealthComponent`, `VisibleComponent`
    и `ShootingComponent`.
-   `Scenes/tower(t2).tscn` --- `HealthComponent`, `VisibleComponent`
    и `ShootingComponent`. 
-   `Scenes/tower(t3).tscn` --- `HealthComponent` и `ExplosiveComponent`.
-   `Scenes/enemie_tower(t1).tscn` --- `MoveComponent`, `HealthComponent`,
    `VisibleComponent` и `ShootingComponent`.
-   `Scenes/enemy_tower(t2).tscn` --- `MoveComponent`, `HealthComponent`,
    `VisibleComponent` и `ShootingComponent`.
-   `Scenes/enemy_tower(t3).tscn` --- `MoveComponent`, `HealthComponent`,
    `ExplosiveComponent` и `VisibleComponent`.
-   `Scenes/player.tscn` --- `HealthComponent` и `PlayersShootingComponent`.
    Скрипт `Player` подключён к корневому узлу сцены.
-   `Scenes/Core.tscn` --- `HealthComponent`. Скрипт `Core` подключён
    к корневому узлу сцены.
-   `Scenes/bullet.tscn` --- `DamagingComponent`. Скрипт `Bullet` подключён
    к корневому узлу сцены.
-   `Scenes/time_bullet.tscn` --- `SlowingComponent`. Скрипт `Bullet`
    подключён к корневому узлу сцены.
-   `Scenes/testTower.tscn` --- дочерних игровых компонентов со скриптами
    нет.
-   `Scenes/DebrisScene.tscn` --- дочерних игровых компонентов со скриптами
    нет; содержит `MeshInstance3D` и `Area3D` с дочерним `CollisionShape3D`.

