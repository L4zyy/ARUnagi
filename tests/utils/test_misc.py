import unittest

import torch
import torchvision

from aru.utils import misc as util

class TestMiscUtils(unittest.TestCase):
    def setUp(self):
        self.model = torchvision.models.resnet18()
    
    def test_parameter_counting(self):
        print(util.count_parameters(self.model))


if __name__ == "__main__":
    unittest.main()