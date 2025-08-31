﻿using PurrNet.Modules;
using PurrNet.Pooling;

namespace PurrNet.Packing
{
    public static class MyersPackDisposableLists
    {
        [UsedByIL]
        public static bool WriteDisposableDeltaList<T>(BitPacker packer, DisposableList<T> old, DisposableList<T> value)
        {
            var scope = new DeltaWritingScope(packer);

            if (Packer.AreEqual(old, value))
                return scope.Complete();

            if (value.isDisposed)
            {
                scope.Write<bool>(false);
                return scope.Complete();
            }

            scope.Write<bool>(true);

            DisposableList<DiffOp<T>> changes;

            if (old.isDisposed)
            {
                using var tmp = DisposableList<T>.Create();
                changes = MyersDiff.Diff(tmp, value);
            }
            else changes = MyersDiff.Diff(old, value);

            if (changes.Count > 0)
            {
                int count = changes.Count;
                for (int i = 0; i < count; i++)
                    scope.Write<DiffOp<T>>(changes[i]);
            }

            scope.Write(DiffOp<T>.FinalOperation());
            return scope.Complete();
        }

        [UsedByIL]
        public static void ReadDisposableDeltaList<T>(BitPacker packer, DisposableList<T> old, ref DisposableList<T> value)
        {
            if (!DeltaReadingScope.Continue(packer, old, ref value))
                return;

            bool hasValue = Packer<bool>.Read(packer);

            if (!hasValue)
            {
                value.Dispose();
                return;
            }

            if (value.isDisposed || (!old.isDisposed && old.list == value.list))
                value = DisposableList<T>.Create();

            if (!old.isDisposed)
            {
                value.Clear();
                value.AddRange(old);
            }

            var changes = DisposableList<DiffOp<T>>.Create();
            while (true)
            {
                var operation = Packer<DiffOp<T>>.Read(packer);
                if (operation.type == OperationType.End)
                {
                    operation.Dispose();
                    break;
                }
                changes.Add(operation);
            }

            if (changes.Count > 0)
            {
                MyersDiff.Apply(value, changes);
                for (var i = 0; i < changes.Count; i++)
                    changes[i].Dispose();
            }

            changes.Dispose();
        }
    }
}
