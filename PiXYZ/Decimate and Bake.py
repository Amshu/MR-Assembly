modelID = scene.getSelectedOccurrences()

algo.decimateTarget(modelID, ["ratio", 40.000000], 100.000000, 1.000000, 100.000000, 10.000000, 10.000000, True, True)
# algo.decimateTarget(modelID, ["triangleCount",10000], 100.000000, 1.000000, 100.000000, 10.000000, 10.000000, True, True)

# decimatetargetbake.decimateTargetBake(modelID, ["ratio",50], pxz.decimatetargetbake.BakeOptions(2048, 1, pxz.decimatetargetbake.BakeMaps(True, True, True, True, False, True, False, False)), True)