modelID = scene.getSelectedOccurrences()

# Rename all children to parent name
# for part in modelID:
#		parent = scene.getParent(part)
#		core.setProperty(part, "Name", core.getProperty(parent, "Name")) 

# Get level 1 Children
_c1List = scene.getChildren(modelID[0])

# Check if there are any children and if not merge parts by name
if len(_c1List) != 0:
		scene.mergePartsByName(modelID[0], 2)

# Loop though the 1st level children
for x in _c1List:
	_c2List = scene.getChildren(x)
	
	# Check if there are any children and if not merge parts by name
	if len(_c2List) != 0:
		scene.mergePartsByName(x, 2)
		break
	
	# Loop through 2nd level
	for y in _c2List: 
		_c3List = scene.getChildren(y)
		
		# Check if there are any children and if not merge parts by name
		if len(_c3List) != 0:
			scene.mergePartsByName(y, 2)
			break
		
		# Loop through 3rd level
		for z in _c3List:
			_c4List = scene.getChildren(z)
			
			# Check if there are any children and if not merge parts by name
			if len(_c4List) != 0:
				scene.mergePartsByName(z, 2)
				break


# Rake and Compress Tree
scene.rake(modelID[0])
scene.compress(modelID[0])
