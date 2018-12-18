using System;
using System.Reflection;

namespace LaboratoryWork6
{
    delegate double PlusOrMinus(double x, double y);

    class Program
    {
        static double Plus(double x, double y) { return x + y; }
        static double Minus(double x, double y) { return x - y; }

        static void PlusOrMinusMethodFunc(string str, double a, double b, Func<double, double, double> PlusOrMinusParam)
        {
            double Result = PlusOrMinusParam(a, b);
            Console.WriteLine(str + Result.ToString());
        }

        static void PlusOrMinusMethod(string str, double i1, double i2, PlusOrMinus PlusOrMinusParam)
        {
            double Result = PlusOrMinusParam(i1, i2);
            Console.WriteLine(str + Result.ToString());
        }


        public static bool GetPropertyAttribute(PropertyInfo checkType, Type attributeType, out object attribute)
        {
            bool Result = false;
            attribute = null;

            //Поиск атрибутов с заданным типом       
            var isAttribute = checkType.GetCustomAttributes(attributeType, false);
            if (isAttribute.Length > 0)
            {
                Result = true;
                attribute = isAttribute[0];
            }

            return Result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("\nДЕЛЕГАТЫ\n");

            double i1 = 1, i2 = 2;

            PlusOrMinusMethod("1 + 2 = ", i1, i2, Plus);
            PlusOrMinusMethod("1 - 2 = ", i1, i2, Minus);

            Console.WriteLine("\n");

            // Создание экземпляра делегата на основе метода
            PlusOrMinus pm1 = new PlusOrMinus(Plus);
            PlusOrMinusMethod("Создание экземпляра делегата на основе метода: 1 + 2 = ", i1, i2, pm1);

            // Создание экземпляра делегата на основе 'предположения' делегата        
            // Компилятор 'пердполагает' что метод Plus типа делегата             
            PlusOrMinus pm2 = Plus;
            PlusOrMinusMethod("Создание экземпляра делегата на основе 'предположения' делегата: 1 + 2 = ", i1, i2, pm2);

            // Создание анонимного метода
            PlusOrMinus pm3 = delegate (double param1, double param2)
            {
                return param1 + param2;
            };
            PlusOrMinusMethod("Создание экземпляра делегата на основе анонимного метода: 1 + 2 = ", i1, i2, pm2);
            PlusOrMinusMethod("Создание экземпляра делегата на основе лямбдавыражения 1: 1 + 2 = ", i1, i2,
                (double x, double y) =>
                {
                    return x + y;
                });
            PlusOrMinusMethod("Создание экземпляра делегата на основе лямбдавыражения 2: 1 + 2 = ", i1, i2, (x, y) => { return x + y; });
            PlusOrMinusMethod("Создание экземпляра делегата на основе лямбдавыражения 3: 1 + 2 = ", i1, i2, (x, y) => x + y);

            Console.WriteLine("\nИспользование обощенного делегата Func<>");

            PlusOrMinusMethodFunc("Создание экземпляра делегата на основе метода: 1 + 2 = ", i1, i2, Plus);

            string OuterString = "ВНЕШНЯЯ ПЕРЕМЕННАЯ";

            PlusOrMinusMethodFunc("Создание экземпляра делегата на основе лямбдавыражения 1: 1 + 2 = ", i1, i2,
                (double x, double y) =>
                {
                    Console.WriteLine("Эта переменная объявлена вне лямбдавыражения: " + OuterString);
                    return x + y;
                });
            PlusOrMinusMethodFunc("Создание экземпляра делегата на основе лямбдавыражения 2: 1 + 2 = ", i1, i2,
                (x, y) =>
                {
                    return x + y;
                });
            PlusOrMinusMethodFunc("Создание экземпляра делегата на основе лямбдавыражения 3: 1 + 2 = ", i1, i2, (x, y) => x + y);

            // Групповой делегат всегда возвращает значение типа void  
            Console.WriteLine("\nПример группового делегата");
            Action<int, int> a1 = (x, y) => { Console.WriteLine("{0} + {1} = {2}", x, y, x + y); };
            Action<int, int> a2 = (x, y) => { Console.WriteLine("{0} - {1} = {2}", x, y, x - y); };
            Action<int, int> group = a1 + a2;
            group(5, 3);

            Action<int, int> group2 = a1;
            Console.WriteLine("\nДобавление вызова метода к групповому делегату");
            group2 += a2;
            group2(10, 5);

            Console.WriteLine("\nУдаление вызова метода из группового делегата");
            group2 -= a1;
            group2(20, 10);


            // ЧАСТЬ 2

            Console.WriteLine("\nРЕФЛЕКСИЯ\n");

            Type t = typeof(ForInspection);

            Console.WriteLine("Тип " + t.FullName + " унаследован от " + t.BaseType.FullName);
            Console.WriteLine("Пространство имен " + t.Namespace);
            Console.WriteLine("Находится в сборке " + t.AssemblyQualifiedName);

            Console.WriteLine("\nКонструкторы:");
            foreach (var x in t.GetConstructors())
            {
                Console.WriteLine(x);
            }

            Console.WriteLine("\nМетоды:");
            foreach (var x in t.GetMethods())
            {
                Console.WriteLine(x);
            }

            Console.WriteLine("\nСвойства:");
            foreach (var x in t.GetProperties())
            {
                Console.WriteLine(x);
            }

            Console.WriteLine("\nПоля данных (public):");
            foreach (var x in t.GetFields())
            {
                Console.WriteLine(x);
            }

            Console.WriteLine("\nСвойства, помеченные атрибутом:");
            foreach (var x in t.GetProperties())
            {
                if (GetPropertyAttribute(x, typeof(NewAttribute), out object attrObj))
                {
                    NewAttribute attr = attrObj as NewAttribute;
                    Console.WriteLine(x.Name + " - " + attr.Description);
                }
            }

            Console.WriteLine("\nВызов метода:");
            // Создание объекта через рефлексию         
            ForInspection fi = (ForInspection)t.InvokeMember(null, BindingFlags.CreateInstance, null, null, new object[] { });

            // Параметры вызова метода 
            object[] parameters = new object[] { 3, 2 };
            // Вызов метода      
            object Result = t.InvokeMember("Plus", BindingFlags.InvokeMethod, null, fi, parameters);
            Console.WriteLine("Plus(3,2)={0}", Result);

            Console.ReadLine();
        }
    }
}

