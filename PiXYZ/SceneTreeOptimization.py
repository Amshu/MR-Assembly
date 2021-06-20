modelID = scene.getSelectedOccurrences()

# Rake Tree
#scene.rake(modelID)

# Merge all parts
#_ret_ = scene.mergeParts(modelID, 2)
# scene.mergePartsByName(modelID, 2)

# Record the name of occurence
# _name = scene.getNodeName(modelID)

# Get Children
_occList = scene.getChildren(modelID[0])

for x in _occList:
	scene.mergePartsByName(x, 2)
	


