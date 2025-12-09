using UnityEngine;
using System.Collections.Generic;

public static class GameSession
{
    // Словари для хранения данных (это как таблички в памяти)
    private static Dictionary<string, int> intData = new Dictionary<string, int>();
    private static Dictionary<string, float> floatData = new Dictionary<string, float>();

    // --- АНАЛОГИ МЕТОДОВ PlayerPrefs ---

    // 1. Сохранить целое число (для дверей и боссов)
    public static void SetInt(string key, int value)
    {
        if (intData.ContainsKey(key)) intData[key] = value;
        else intData.Add(key, value);
    }

    // Получить целое число
    public static int GetInt(string key)
    {
        if (intData.ContainsKey(key)) return intData[key];
        return 0; // По умолчанию 0
    }

    // 2. Сохранить дробное число (для координат чекпойнта)
    public static void SetFloat(string key, float value)
    {
        if (floatData.ContainsKey(key)) floatData[key] = value;
        else floatData.Add(key, value);
    }

    // Получить дробное число
    public static float GetFloat(string key)
    {
        if (floatData.ContainsKey(key)) return floatData[key];
        return 0f;
    }

    // 3. Очистить всё (для кнопки "Новая игра")
    public static void ClearAll()
    {
        intData.Clear();
        floatData.Clear();
    }
}