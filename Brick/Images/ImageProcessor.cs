namespace Brick.Images {
    public interface ImageProcessor {
        string ResizeImage(string image64, int maxWidth, int maxHeight);
    }
}