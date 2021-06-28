modelID = scene.getSelectedOccurrences()

# Get level 1 Children
_c1List = scene.getChildren(modelID[0])

# Loop though the 1st level children
for x in _c1List:
	_ret_ = generateproxy.proxyFromMeshes(100, ["Yes",pxz.generateproxy.BakeOptions(4096, 1, pxz.generateproxy.BakeMaps(True, True, True, True, True, True, True, False))], True, True)