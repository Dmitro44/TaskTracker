export function getContrastTextColor(hexColor: string | undefined | null): string {
    if (!hexColor || !hexColor.startsWith("#") || (hexColor.length !== 4 && hexColor.length !== 7)) {
        // If HEX color invalid
        return 'black';
    }

    let rStr, gStr, bStr;

    if (hexColor.length === 4) { // короткий формат #RGB
        rStr = hexColor.slice(1, 2) + hexColor.slice(1, 2);
        gStr = hexColor.slice(2, 3) + hexColor.slice(2, 3);
        bStr = hexColor.slice(3, 4) + hexColor.slice(3, 4);
    } else { // полный формат #RRGGBB
        rStr = hexColor.slice(1, 3);
        gStr = hexColor.slice(3, 5);
        bStr = hexColor.slice(5, 7);
    }

    const r = parseInt(rStr, 16);
    const g = parseInt(gStr, 16);
    const b = parseInt(bStr, 16);

    // Формула для определения яркости (YIQ)
    const brightness = (r * 299 + g * 587 + b * 114) / 1000;
    return brightness > 128 ? 'black' : 'white';
}