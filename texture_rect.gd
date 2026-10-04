extends TextureRect


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$P1Character.texture = load("res://art/characters/%s.png" % GameState.P1Character)
	pass # Replace with function body.
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
