modelID = scene.getSelectedOccurrences()

# Repair CAD
#algo.repairCAD(modelID, 0.100000, True)

# Tesselate - First three params (0.5, 100, -1)
#algo.tessellate([1], 0.500000, -1, -1, True, 2, 1, 0.000000, False, False, True, False)

# Repair Mesh
algo.repairMesh(modelID, 0.100000, True, True)

# Smart Orient
algo.smartOrient(modelID, 500.000000, 1.000000, 512, 1, False, 1)

# Fix Normals
scene.deleteNormals()
scene.createNormals()
scene.orientNormals()

