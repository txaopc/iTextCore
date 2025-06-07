// Copyright 2013 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// This is part of java port of project hosted at https://github.com/google/woff2
namespace iText.IO.Font.Woff2 {
    // Helper functions for storing integer values into byte streams.
    // No bounds checking is performed, that is the responsibility of the caller.
    internal class StoreBytes {
        public static int StoreU32(byte[] dst, int offset, int x) {
            dst[offset] = JavaUnsignedUtil.ToU8(x >> 24);
            dst[offset + 1] = JavaUnsignedUtil.ToU8(x >> 16);
            dst[offset + 2] = JavaUnsignedUtil.ToU8(x >> 8);
            dst[offset + 3] = JavaUnsignedUtil.ToU8(x);
            return offset + 4;
        }

        public static int StoreU16(byte[] dst, int offset, int x) {
            dst[offset] = JavaUnsignedUtil.ToU8(x >> 8);
            dst[offset + 1] = JavaUnsignedUtil.ToU8(x);
            return offset + 2;
        }
    }
}
