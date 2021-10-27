modelID = scene.getSelectedOccurrences()

# Hidden Removal
_ret_ = algo.hiddenRemoval(modelID, 0, 4096, 16, 90.000000, False, 1)

# Smart Hidden Removal
_ret_ = algo.smartHiddenRemoval(modelID, 2, 1000.000000, 1.000000, 1024, 1, False, 1)