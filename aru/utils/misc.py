import torch
import torchvision

from torch import nn
from torchvision import transforms as T

inv_normalize = T.Normalize(
    mean=[-0.485/0.229, -0.456/0.224, -0.406/0.225],
    std=[1/0.229, 1/0.224, 1/0.225]
)
sample2img = T.Compose([
    inv_normalize,
    T.ToPILImage(),
])

def count_parameters(model: nn.Module):
    return sum(p.numel() for p in model.parameters() if p.requires_grad)