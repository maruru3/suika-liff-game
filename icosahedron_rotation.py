import numpy as np
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from matplotlib.animation import FuncAnimation

def create_icosahedron():
    phi = (1 + np.sqrt(5)) / 2

    vertices = np.array([
        [0, 1, phi],
        [0, -1, phi],
        [0, 1, -phi],
        [0, -1, -phi],
        [1, phi, 0],
        [-1, phi, 0],
        [1, -phi, 0],
        [-1, -phi, 0],
        [phi, 0, 1],
        [-phi, 0, 1],
        [phi, 0, -1],
        [-phi, 0, -1]
    ])

    faces = [
        [0, 1, 8], [0, 8, 4], [0, 4, 5], [0, 5, 9], [0, 9, 1],
        [2, 3, 11], [2, 11, 5], [2, 5, 4], [2, 4, 10], [2, 10, 3],
        [1, 6, 8], [6, 10, 8], [10, 4, 8], [4, 10, 2], [6, 7, 10],
        [7, 11, 10], [11, 3, 10], [3, 7, 11], [7, 9, 11], [9, 5, 11],
        [1, 9, 7], [1, 7, 6], [6, 8, 1], [9, 7, 1], [5, 9, 11]
    ]

    return vertices, faces

def rotation_matrix(angle_x, angle_y, angle_z):
    Rx = np.array([[1, 0, 0],
                   [0, np.cos(angle_x), -np.sin(angle_x)],
                   [0, np.sin(angle_x), np.cos(angle_x)]])

    Ry = np.array([[np.cos(angle_y), 0, np.sin(angle_y)],
                   [0, 1, 0],
                   [-np.sin(angle_y), 0, np.cos(angle_y)]])

    Rz = np.array([[np.cos(angle_z), -np.sin(angle_z), 0],
                   [np.sin(angle_z), np.cos(angle_z), 0],
                   [0, 0, 1]])

    return Rz @ Ry @ Rx

def animate(frame):
    ax.clear()

    angle_x = frame * 0.02
    angle_y = frame * 0.03
    angle_z = frame * 0.01

    R = rotation_matrix(angle_x, angle_y, angle_z)
    rotated_vertices = vertices @ R.T

    for face in faces:
        triangle = rotated_vertices[face]
        ax.plot_trisurf(triangle[:, 0], triangle[:, 1], triangle[:, 2],
                       alpha=0.7, shade=True, color='cyan', edgecolor='black', linewidth=0.5)

    ax.set_xlim([-3, 3])
    ax.set_ylim([-3, 3])
    ax.set_zlim([-3, 3])
    ax.set_xlabel('X')
    ax.set_ylabel('Y')
    ax.set_zlabel('Z')
    ax.set_title('正十面体 (Icosahedron) - 回転中')

vertices, faces = create_icosahedron()

fig = plt.figure(figsize=(10, 8))
ax = fig.add_subplot(111, projection='3d')

ani = FuncAnimation(fig, animate, frames=1000, interval=50, repeat=True)

plt.show()