using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ClangReader.Utilities
{
    internal class FastReflection
    {
        public class Field<TClassType>
        {
            public static Action<TClassType, TFieldType> CreateSetter<TFieldType>(Expression<Func<TClassType, TFieldType>> fieldAccessor)
            {
                if (!(GetMemberInfo(fieldAccessor) is FieldInfo fieldInfo))
                {
                    throw new ArgumentException("Input is not a field", nameof(fieldAccessor));
                }

                return FastReflection.CreateFieldSetter<TClassType, TFieldType>(fieldInfo);
            }

            public static Func<TClassType, TFieldType> CreateGetter<TFieldType>(Expression<Func<TClassType, TFieldType>> fieldAccessor)
            {
                if (!(GetMemberInfo(fieldAccessor) is FieldInfo fieldInfo))
                {
                    throw new ArgumentException("Input is not a field", nameof(fieldAccessor));
                }

                return FastReflection.CreateFieldGetter<TClassType, TFieldType>(fieldInfo);
            }
        }

        public static Action<TClassType, TFieldType> CreateFieldSetter<TClassType, TFieldType>(FieldInfo field)
        {
            var dmName = string.Concat("_Set", field.Name, "_");
            var dm = new DynamicMethod
            (
                name: dmName,
                returnType: typeof(void),
                parameterTypes: new[] { typeof(TClassType), typeof(TFieldType) },
                owner: field.DeclaringType,
                skipVisibility: true
            );

            var generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);

            generator.Emit(OpCodes.Stfld, field);
            generator.Emit(OpCodes.Ret);

            return (Action<TClassType, TFieldType>)dm.CreateDelegate(typeof(Action<TClassType, TFieldType>));
        }

        public static Func<TClassType, TFieldType> CreateFieldGetter<TClassType, TFieldType>(FieldInfo field)
        {
            var dmName = string.Concat("_Get", field.Name, "_");
            var dm = new DynamicMethod
            (
                name: dmName,
                returnType: typeof(TFieldType),
                parameterTypes: new[] { typeof(TClassType) },
                owner: field.DeclaringType,
                skipVisibility: true
            );

            var generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            generator.Emit(OpCodes.Ldfld, field);
            generator.Emit(OpCodes.Ret);

            return (Func<TClassType, TFieldType>)dm.CreateDelegate(typeof(Func<TClassType, TFieldType>));
        }

        //Todo: move
        private static MemberInfo GetMemberInfo<T, T2>(Expression<Func<T, T2>> accessor)
        {
            var lambda = accessor as LambdaExpression;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression unaryExpression)
            {
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            return memberExpression?.Member;
        }
    }
}