modelID = scene.getSelectedOccurrences()

algo.removeUV(modelID, -1)

algo.mapUvOnBox(modelID, pxz.algo.Box(pxz.geom.Affine(pxz.geom.Point3(428.542,1081.32,-155.4), pxz.geom.Point3(0,0,-435.2), pxz.geom.Point3(0,507.43,0), pxz.geom.Point3(259.084,0,0)), 217.600000, 253.715000, 129.542000), 0, True)

_ret_ = algo.repackUV(modelID, 0, True, 2048, 2, False, 3, True)

