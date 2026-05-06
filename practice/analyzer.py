import json
import sys
import os

INPUT_FILE = "input.json"
OUTPUT_FILE = "output.json"

def calculate(numbers):
    return {
        "sum": sum(numbers),
        "average": sum(numbers) / len(numbers),
        "min": min(numbers),
        "max": max(numbers),
        "count": len(numbers)
    }

if __name__ == "__main__":
    if not os.path.exists(INPUT_FILE):
        print(f"Файл {INPUT_FILE} не найден", file=sys.stderr)
        sys.exit(1)

    with open(INPUT_FILE, "r") as f:
        data = json.load(f)

    numbers = data.get("numbers", [])

    if not numbers:
        print("Список чисел пустой", file=sys.stderr)
        sys.exit(2)

    result = calculate(numbers)

    with open(OUTPUT_FILE, "w") as f:
        json.dump(result, f, indent=2)

    print("OK")
