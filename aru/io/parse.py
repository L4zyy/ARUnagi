# borrow from pytorch3d source code https://github.com/facebookresearch/pytorch3d/blob/eb04a488c57da0e4e5651f5f37349e058cbfedc7/pytorch3d/io/obj_io.py#L415

from pathlib import Path

def parse_face(
    line,
    tokens,
    materials_idx,
    faces_verts_idx,
    faces_normals_idx,
    faces_textures_idx,
    faces_materials_idx,
):
    face = tokens[1:]
    face_list = [f.split('/') for f in face]
    face_verts = []
    face_normals = []
    face_textures = []

    for vert_props in face_list:
        # Vertex index.
        face_verts.append(int(vert_props[0]))
        if len(vert_props) > 1:
            if vert_props[1] != "":
                face_textures.append(int(vert_props[1]))
            if len(vert_props) > 2:
                face_normals.append(int(vert_props[2]))
            if len(vert_props) > 3:
                msg = "Face vertices can only have 3 properties. Face vert %s, Line: %s"
                raise ValueError(msg % (str(vert_props), str(line)))

    # Triplets must be consistent for all vertices in a face e.g.
    # legal statement: f 4/1/1 3/2/1 2/1/1.
    # illegal statement: f 4/1/1 3//1 2//1.
    # If the face does not have normals or textures indices
    # fill with pad value = -1. This will ensure that
    # all the face index tensors will have F values where
    # F is the number of faces.
    if len(face_normals) > 0:
        if not (len(face_verts) == len(face_normals)):
            msg = "Face %s is an illegal statement. Vertex properties are inconsistent. Line: %s"
            raise ValueError(msg % (str(face), str(line)))
    else:
        face_textures = [-1] * len(face_verts)

    # Subdivide faces with more than 3 vertices.
    for i in range(len(face_verts) - 2):
        faces_verts_idx.append((face_verts[0], face_verts[i+1], face_verts[i+2]))
        if len(face_normals):
            faces_normals_idx.append((face_normals[0], face_normals[i+1], face_normals[i+2]))
        if len(face_textures):
            faces_textures_idx.append((face_textures[0], face_textures[i+1], face_textures[i+2]))
        faces_materials_idx.append(materials_idx)

def parse_obj(f, parent):
    verts, normals, verts_uvs = [], [], []
    faces_verts_idx, faces_normals_idx, faces_textures_idx = [], [], []
    faces_materials_idx = []
    material_names = []
    mtl_path = None

    lines = [line.strip() for line in f]

    # If the file is read in as bytes then first decode to strings.
    if lines and isinstance(lines[0], bytes):
        lines = [el.decode('utf-8') for el in lines]

    materials_idx = -1
    
    for line in lines:
        tokens = line.strip().split()

        if line.startswith('mtllib'):
            if len(tokens) < 2:
                raise ValueError("Material file name is not specified.")
            
            mtl_path = line[len(tokens[0]):].strip()
            mtl_path = parent / mtl_path
        elif len(tokens) and tokens[0] == 'usemtl':
            material_name = tokens[1]
            if material_name not in material_names:
                material_names.append(material_name)
                materials_idx = len(material_names) - 1
            else:
                materials_idx = material_names.index(material_name)
        elif line.startswith('v '):     # Line is a vertex.
            vert = [float(x) for x in tokens[1:4]]
            if len(vert) != 3:
                msg = "Vertex %s does not have 3 values. Line: %s"
                raise ValueError(msg % (str(vert), str(line)))
            verts.append(vert)
        elif line.startswith('vt '):    # Line is a texture.
            tx = [float(x) for x in tokens[1:3]]
            if len(tx) != 2:
                msg = "Texture %s does not have 2 values. Line: %s"
                raise ValueError(msg % (str(tx), str(line)))
            verts_uvs.append(tx)
        elif line.startswith('vn '):    # Line is a normal.
            norm = [float(x) for x in tokens[1:4]]
            if len(norm) != 3:
                msg = "Normal %s does not have 3 values. Line: %s"
                raise ValueError(msg % (str(norm), str(line)))
            normals.append(norm)
        elif line.startswith('f '):     # Line is a face.
            parse_face(
                line,
                tokens,
                materials_idx,
                faces_verts_idx,
                faces_normals_idx,
                faces_textures_idx,
                faces_materials_idx,
            )
    
    return (
        verts,
        normals,
        verts_uvs,
        faces_verts_idx,
        faces_normals_idx,
        faces_textures_idx,
        faces_materials_idx,
        material_names,
        mtl_path,
    )