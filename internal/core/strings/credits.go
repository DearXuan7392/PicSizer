package strs

import _ "embed"

// CreditsHTML 第三方开源组件声明的 HTML 内容, 用于在 WebView 中展示.
//
//go:embed embed/credits.html
var CreditsHTML string
