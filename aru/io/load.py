from collections import namedtuple
from pathlib import Path

import parse

supported_file_type = ['.obj']

def load(file):
    file = Path(file)

    if file.suffix in supported_file_type:
        with open(file, mode='r') as f:
            (
                verts,
                normals,
                verts_uvs,
                faces_verts_idx,
                faces_normals_idx,
                faces_textures_idx,
                faces_materials_idx,
                material_names,
                mtl_path,
            ) = eval('parse.parse_' + file.suffix[1:] + '(f, file.parent)')
            print(len(verts), verts[0])
    else:
        print('Unsupported file format.')

load("./bunny.obj")
