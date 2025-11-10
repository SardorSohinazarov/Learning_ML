# Learning_ML

### Hozir ko'p narsa yuklanib keyin amal bajarilyapti

```
private static float Cosine(float[] a, float[] b)
{
    double dot = 0, na = 0, nb = 0;
    for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
    if (na == 0 || nb == 0) return 0f;
    return (float)(dot / (Math.Sqrt(na) * Math.Sqrt(nb)));
}
    ```

shu formulani sql da yozib chiqolsak kam yuklanish bilan ham eplasha bo'larkan