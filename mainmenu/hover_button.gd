extends TextureButton

func _ready() -> void:
	mouse_entered.connect(func(): self_modulate = Color(0.75, 0.75, 0.75))
	mouse_exited.connect(func(): self_modulate = Color.WHITE)
	button_down.connect(func(): self_modulate = Color(0.6, 0.6, 0.6))
	button_up.connect(func(): self_modulate = Color(0.75, 0.75, 0.75))
	pressed.connect(func(): get_tree().change_scene_to_file("res://levels/song_level/song_level.tscn"))
	
