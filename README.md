# Hex Grid
A hexagonal grid implementation based on the following documentation:
- [Hexagonal Grids](https://www.redblobgames.com/grids/hexagons/),
- [Hexmod representation](https://observablehq.com/@sanderevers/hexmod-representation).

This project is meant to be used with the [Godot Engine](https://godotengine.org/), but can be easily modified
to work with any rendering backend. The implementation is divided into map and entity, 
each having its own source and tests projects.
The fifth project contains an editor tool to edit the map and manually test features.

## HexGrid.Map
The core consists of:
- GridMap - the main interface to interact with the map,
- Layer - an abstraction which envelops a vertical slice of the map. Houses data storage and rendering,
- Properties - a singleton containing critical map settings. 
An instance has to be created before creating a GridMap object.
### Vector
The chosen coordinate system is cube coordinates. This mean that each hexagonal direction has its own
coordinate component. It was chosen instead of axial, because it requires the least amount
of conversions for different features and is explicit.

Other than an integer implementation, there is a float implementation, which is used
for conversion and extensions methods.
### Hex
Hex is a single cell in the map. It stores its position, logic properties and mesh data.
While the hexes themselves can't be rotated, the meshes can.

Those hexes are serialized in IHexMapData, which is to be implemented by the user.
HexMap manages the process of serialization and deserialization. It also allows for
addition, removal and retrieval of single hexes. It's also possible to append map data
to a map and retrieve the whole map.
### Renderer
Renderers are an abstraction for rendering (obviously). A renderer module has to have:
- a renderer, responsible for display,
- a map, to store and manage renderers,
- a factory, which creates maps. This is used when creating a new layer.
#### Chunk
Initially the whole grid was meant to be purely virtual, rendered directly by the RenderingServer, using
mesh instancing. Chunks are this initial idea, which is simple but limited.

A chunk is a set size and renders every hex in its range. It doesn't use any mesh instance nodes. Instead, it
talks directly with the RenderingServer and tracks its multi mesh instances.

Each chunk has a position, which can be calculated from any hex that's within its size.
#### Room
Rooms are the second iteration of chunks, but not fully finished yet. They came to be because of particles.
In Godot, CPU particles don't support collisions and GPU particles have to have special collision nodes to collide with.
It came down to SDF collision nodes, which bake a collision texture based on mesh instances in range. Chunks don't
use mesh instances, so rooms where born.

A room combines assigned hexes into as few meshes as possible, which are than assigned to mesh instances. You could
use multi mesh instances, but a quick glance at the documentation tells you that SDF baking only works with
mesh instances... This is the main difference between chunks and rooms.
### Mesh
There are two simple direct mesh renderers:
- WireMesh - as the name suggests it creates a hexagonal wire mesh, which is notably used as an indicator in the editor,
- PrimitiveMesh - it renders solid hexagonal faces in passed HexVector positions.
### Fov
Low priority module, currently only has a line field of view implementation, which draws a line to a target and
checks if an occluder hex is in the way.
## HexGrid.Entity
An entity is a base object which can reside in the grid, with the current implementation. It houses providers, which
are responsible for logic, and handlers, which listen to providers and can for example move the entity in the engine.
### Providers
A provider is responsible for logic behaviour of an entity and doesn't concern itself with display.
There are currently three basic providers for:
- position - manages entity movement including whether it can move to a target. Doesn't consider rotation and
moves using an offset from its current location,
- rotation - manages the orientation of an entity,
- block - allows an entity to block directions. It's used to block entry or exit from/to a hex.
### Handlers
A handler listens for a change in a provider and acts accordingly. There are two handlers:
- position - handles object translation in the engine. Currently, there is an instant and linear handler,
- rotation - handles object rotation in the engine. There is only an instant handler.
### Managers
The main class here is an EntityManager, which houses a layer manager for each layer in the grid map. 
A layer manager stores Entity stacks, the same way hexes are stored. Each stack contains a list of entities currently
in this stack, which translates to a list of entities on a hex.

Managers provide the ability to retrieve data, add or remove entities and update their state.
## Godot Tools
The tool contains:
- a hex editor,
- an input handler,
- a chunk visualization module,
- a wire mesh hex indicator,
- a field of view visualization module.

### Hex editor
Directly edits the hexes on a map. It uses the Hexes tab where it provides:
- clearing the current layer,
- clearing the whole map,
- selecting the current layer,
- modifying hex properties (like height and if it's an occluder),
- a list of available meshes in the MeshLibrary - allows for selecting a mesh as a brush.

It uses the input handlers event to perform:
- selected mesh addition - by left mouse button press or drag,
- selected hex deletion - by right mouse button press or drag,
- selected mesh update to cursor position - by mouse motion in the viewport,
- temporary display of all layers - by space bar press and reversed by any other action,
- deselection of selected mesh - by escape button press,
- pipette selected mesh and hex properties - by Q button press.

### Input handler
Interprets the input from the plugin and provides events for specific actions:
- rotation - when R key is pressed and a mesh is selected,
- pipette - when Q key is pressed,
- deselect - when escape key is pressed,
- display all - when space is pressed,
- hex center updated - when the pointer changes which hex it's pointing to,
- add hex - on left mouse button pressed and a mesh is selected,
- remove hex - on right mouse button pressed and a mesh is selected.
### Chunk visualization module
Uses the position from the input provider, converted to chunk position, to render a WireMesh using the size of 
a chunk in the properties singleton. This can be enabled in the settings tab of the editor view.
### Wire mesh hex indicator
Uses the position from the input provider to render a WireMesh around that position with a specified range in the
settings tab of the editor view.
### Field of view visualization module
Retrieves the visible hexes and renders a PrimitiveMesh with a specified range in the settings tab of the editor view,
where it can also be enabled.