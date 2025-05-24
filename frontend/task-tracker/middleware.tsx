import { NextRequest, NextResponse } from "next/server";

const protectedRoutes = ["/dashboard", "/b/"];

export async function middleware(request: NextRequest) {
    const { pathname } = request.nextUrl;

    if (
        pathname.startsWith("/login") ||
        pathname.startsWith("/register") ||
        pathname.startsWith("/_next") ||
        pathname.startsWith("/api")
    ) {
        return NextResponse.next();
    }

    const isProtected = protectedRoutes.some(route => pathname.startsWith(route));
    if (!isProtected) {
        return NextResponse.next();
    }

    const cookie = request.headers.get("cookie") ?? "";

    try {
        const meRes = await fetch("https://localhost:7165/api/Auth/me", {
            method: "GET",
            headers: { cookie },
        });

        if (meRes.status !== 200) {
            const loginUrl = new URL("/login", request.url);
            loginUrl.searchParams.set("from", pathname);
            return NextResponse.redirect(loginUrl);
        }
    } catch (err) {
        console.error("Ошибка запроса к backend в middleware:", err);
        // можно редиректить на логин или пускать дальше (например, в dev)
        const loginUrl = new URL("/login", request.url);
        loginUrl.searchParams.set("from", pathname);
        return NextResponse.redirect(loginUrl);
    }

    return NextResponse.next();
}

export const config = {
    matcher: [
        "/dashboard/:path*",
        "/b/:path*",
    ]
}